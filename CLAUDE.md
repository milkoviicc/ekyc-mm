# CLAUDE.md – eKYC Development Guide

This file is the primary context document for Claude Code. Read it fully before starting any task.

---

## Project Overview

**eKYC** is the .NET rewrite of a legacy Java/Vaadin 7 AML/KYC compliance platform (`C:\Podhvati\eKYC-2`) used by HBOR (Croatian Bank for Reconstruction and Development) to onboard and periodically re-screen bank clients — individuals, legal entities, and correspondent banks — for money-laundering and terrorist-financing risk.

Backend is .NET 10, database is the **existing** Microsoft SQL Server database (schema kept as-is, not redesigned — see Migration Decisions below). Data access uses Dapper. Two front ends are being built against one shared API: **Blazor Server (primary)** and **React (comparison build)**.

The original migration analysis (architecture read of the legacy app, live schema audit, and all decisions below) was written up as an artifact during planning — ask the user for the link if you need the full narrative; this file has the durable conclusions.

---

## Solution Structure

```
src/
  eKYC.Domain/         ← POCOs only, no logic, no dependencies
  eKYC.DataAccess/      ← Dapper repositories + IDbConnectionFactory (SQL Server)
  eKYC.Application/     ← Services, workflow/state-machine, tenancy
  eKYC.Api/             ← ASP.NET Core REST controllers — both Web projects call this, never DataAccess directly
  eKYC.Web.Blazor/       ← Blazor Server UI (primary)
  eKYC.Web.React/       ← React + TypeScript UI (Vite, Redux Toolkit, Material UI) - comparison build; see its README.md
tests/
  eKYC.UnitTests/        ← xUnit + Moq + FluentAssertions, mocks repositories
  eKYC.IntegrationTests/ ← xUnit + FluentAssertions, hits the REAL dev database — see below
database/
  migrations/            ← Numbered DbUp SQL scripts, additive only (schema is kept as-is)
```

---

## Migration Decisions (locked in — don't re-litigate without asking)

1. **Front end: Blazor Server is primary**, built to full feature parity. React is a separate comparison project, scoped to an early slice only, not chasing full parity.
2. **Strategy: big-bang.** The legacy Vaadin app stays live and untouched until this rewrite reaches full parity, then cuts over once. No phased/parallel-module rollout.
3. **Database schema: kept as-is.** No redesign, no data migration. Dapper repositories map directly to the existing Croatian-abbreviated table/column names (e.g. `CL_Clnt`, `Clnt_Typ_Cd`), aliased in SELECTs to PascalCase C# properties. The only schema changes are **additive**: `row_version` and `tenant_id` columns added per concurrency-controlled table via numbered DbUp migrations (see `database/migrations/001_...`).
4. **SSO: Kerberos/AD preserved**, via `Microsoft.AspNetCore.Authentication.Negotiate` (SPNEGO), matching the legacy app's Windows-integrated login. **Known issue found while verifying this (2026-08-20):** `eKYC.Api`, hosted directly on Kestrel, completes a real Windows-authenticated NTLM/Negotiate handshake correctly (verified with `Invoke-WebRequest -UseDefaultCredentials` — real 200, real data). `eKYC.Web.Blazor`, also directly on Kestrel, fails the *browser-driven* multi-leg handshake with a `400 Bad Request` / `InvalidToken` after 2-3 legs, even after fixing an earlier issue where the 401 challenge carried a full HTML body instead of being empty (fixed by calling `.RequireAuthorization()` on the `MapRazorComponents` endpoint plus an explicit `app.UseRouting()` — keep both, they're correct regardless). The remaining failure looks like a connection-reuse problem specific to Kestrel + Blazor Server's response pipeline, not a config mistake — Negotiate/NTLM requires the *same* TCP connection across every leg of the handshake. **This has not been root-caused further.** Production deployment target is IIS (see below), where Windows Authentication is normally handled by IIS's own module before the request reaches Kestrel/ASP.NET Core at all — test there before assuming this is still a problem; don't sink more time into the Kestrel-hosted case unless IIS turns out to have the same issue.
5. **SafeWatch (Fircosoft) sanctions screening: out of scope for now.** Don't build this integration. The legacy workflow has SAFEWATCH PROVJERA/OK/NOT OK processing-status states — decide with the user whether the new state machine stubs or drops them before building workstream 4 (workflow/approvals).
6. **SharePoint document integration: stays.** Port it, don't retire it.
7. **HBOR client-master data: read via an abstraction, not a direct linked-server query.** The legacy app reads `HBORIS.dbo.[HBOR.vKlijent]` as a cross-database view against a SQL Server linked server. HBOR does not currently expose an API for this data — Trikord will request one, but won't build a facade themselves. **Until that API exists**, implement `IClientMasterDataProvider` (in `eKYC.Application`) with an interim implementation that reads `HBORIS` the same way the legacy app does, so downstream work isn't blocked on HBOR's timeline. Swap in an API-backed implementation later via DI — no other code should need to change.
8. **Deployment: IIS** (Windows Server), same as this workspace's other projects. Docker is a possible future target — avoid IIS-only assumptions where reasonably avoidable, but don't over-engineer for a container story that isn't being built yet.
9. **No written regulatory/AML requirements exist.** Test coverage is driven by the legacy workflow and live schema as reverse-engineered, not an external spec.

---

## Concurrency Patterns

Same three patterns as this workspace's other .NET projects, adapted for SQL Server:

### Pattern 1: Optimistic Locking (`row_version`)
- Additive `row_version INT NOT NULL DEFAULT (1)` column per concurrency-controlled table (added via DbUp migration, not present in the legacy schema).
- Every UPDATE includes `AND row_version = @RowVersion` in the WHERE clause and `row_version = row_version + 1` in the SET clause.
- Repository update methods return `int` (rows affected), never `void`.
- 0 rows affected → the service layer throws `eKYC.Domain.Exceptions.ConcurrencyException<T>` carrying the freshly reloaded current record. See `eKYC.Application.Clients.CL_ClntService.UpdateAsync` for the reference implementation, and `eKYC.Api.Middleware.ExceptionHandlingMiddleware` for how it's mapped to HTTP 409.

### Pattern 2: Pessimistic Locking (edit-session lock)
- Legacy app uses an `A_Object_Locks` table (Object_Id, Object_Class, Object_Name, User_Id, Locked_At, Computer_Name) to block a second user from opening a record someone else is editing.
- **Implemented (2026-10-08), table-based**: `POST api/locks` takes the lock atomically (`UPDLOCK, HOLDLOCK`), `PUT api/locks/{id}` is the heartbeat, `DELETE` releases; a lock not refreshed for `ObjectLocks:TtlMinutes` (30) counts as abandoned and can be taken over. The React edit dialogs use it via `useEditLock`; Blazor does not yet. Admin list/release lives in Administriranje -> Zaključavanje.
- **This is distinct from `row_version` above** — pessimistic locking blocks the second editor up front; optimistic locking catches the conflict at save time. Both are being built; don't conflate them.

### Pattern 3: Idempotent Insert
- `INSERT ... ON CONFLICT` equivalent (`MERGE` or `IF NOT EXISTS` in SQL Server) for any future callback/batch-driven ingestion (e.g. if SafeWatch work resumes). Not currently needed since screening is out of scope.

---

## Data Access Rules (Dapper)

- All SQL lives in `eKYC.DataAccess` repository classes only.
- Parameterized queries exclusively — the legacy Java app builds SQL by string concatenation in places; do not repeat that pattern.
- Naming convention (changed 2026-10-08 at the user's request): the `A_*` admin tables and `CL_Clnt` / `CL_Doc_Revisions` use model classes named exactly like the table, with properties named exactly like the columns (e.g. `A_Usr.Usr_Id`, `CL_Clnt.Clnt_Typ_Cd`), plus matching `I<Table>Repository` / `<Table>Service` / `<Table>Controller` names — so their SELECTs need no aliases. Older models not yet migrated (Dashboard/Reports/ReferenceData rows, filters) still alias to PascalCase; ask before renaming those.
- `IDbConnectionFactory` → `SqlConnectionFactory` (`Microsoft.Data.SqlClient`), injected into repositories, one connection per call.
- **Only 9 of the legacy schema's 50 tables have real FOREIGN KEY constraints, and 3 tables have no PRIMARY KEY at all** (`A_ISRole_St`, `CL_MatPod_Compare`, `CL_Task_Interval_Names`). Referential integrity is enforced by application code, not the database — don't assume cascade behavior, and flag to the user before building against a table with no PK (it needs one added first).

---

## Porting a Legacy Tab/Layout

The legacy app has 7 top-level tabs (`TkLayout*.java` in `Layouts/`): Login, Nadzorna ploča (Dashboard), Klijenti - analiza, Klijenti - pregled, Izvješća, Revizija, Administriranje. **Dashboard is done** (2026-08-21) — use it as the template for porting the rest:

1. Read the legacy `TkLayout*.java` file to identify its sub-forms (filters, buttons) and which `DataTables`/views it binds to.
2. Read the backing SQL view/table definition directly from the DB (`OBJECT_DEFINITION(OBJECT_ID('dbo.ViewName'))`) rather than guessing columns from the Java code.
3. Build full-stack in the usual order: `Domain/<Tab>/*.cs` (row + filter POCOs) → `DataAccess/<Tab>/*Repository.cs` (parameterized SQL, never string-concatenated — the legacy app gets this wrong, don't repeat it) → `Application/<Tab>/*Service.cs` → `Api/Controllers/<Tab>Controller.cs` → `Components/Pages/<Tab>.razor` (MudBlazor: filter sub-form + `MudTable`/`MudTabs` + action buttons).
4. Write integration tests against the real dev DB for any nontrivial join/filter logic — this is how the Dashboard's multi-table join was actually verified correct, not just compiled.
5. If the legacy view reads `HBORIS`/`vKlijent` (the upstream HBOR feed) for data not yet in eKYC's own tables, that part is out of scope until `IClientMasterDataProvider` exists (decision #7) — port only the branch that reads eKYC's own tables, and note the gap explicitly rather than silently dropping it.

## Automating MudBlazor Forms/Dialogs in the Browser Preview Tool

Real users click/type into MudBlazor components fine — this is only about driving them from the automated browser tool during verification:

- **`computer` tool's plain `left_click` often doesn't register with MudBlazor's own JS-managed click handling** (seen on `MudTabs`, `MudSelect` popovers, `MudTextField`). Fix: dispatch a full synthetic event sequence in order — `pointerdown`, `mousedown`, `pointerup`, `mouseup`, `click` — via `javascript_tool`, all with matching `clientX`/`clientY` and `bubbles: true`.
- **A `MudSelect` popover selected via synthetic dispatch may not actually close afterward**, and the still-open (but invisible-looking) popover then silently intercepts clicks on whatever sits underneath it — every subsequent field interaction fails with no obvious cause. Check `document.querySelectorAll('.mud-popover')` for elements with `offsetParent !== null` if later fields won't focus; force-close with `el.style.display = 'none'` to unblock testing (this is a testing-only workaround, not a real user's experience).
- **`computer.type` requires genuine DOM focus first, and plain `left_click` doesn't reliably produce it on `MudTextField`/`MudDatePicker` inputs.** Set focus directly via `element.focus()` in `javascript_tool`, confirm with `document.activeElement === element`, then use `computer.type` for the actual keystrokes.
- **`MudDatePicker`'s text input resists typed date strings** (format parsing is finicky). More reliable: click its calendar "Open" button, then click the target day cell (`button.mud-picker-calendar-day`) directly.
- After filling a form this way, **read back every field's `.value` via `javascript_tool` before submitting** — don't assume typing succeeded just because the `type` action reported success.

## MudTable Row Coloring — Style Every Cell, Not Just the Row

`MudTable`'s `RowStyleFunc` sets an inline `style` on the `<tr>`, but MudBlazor's own table-cell CSS sets an explicit `color` on each `<td>` — and a child's own explicit CSS value is not affected by a parent's inherited value, `!important` or not (inheritance only kicks in when the child doesn't set its own value). Result: `background-color` from `RowStyleFunc` shows up fine, but `color` silently does nothing — text stays whatever MudBlazor's default is, invisible against a dark background.

**Fix:** compute the style string once per row, then pass it to every `MudTd`'s own `Style` parameter too (not just rely on the row). See `Dashboard.razor`/`DashboardRowStyling` for the pattern. Found this the hard way — first pass "worked" (background changed) but text stayed dark-on-dark-red until spot-checked with `getComputedStyle` in the browser, which is the only way to actually catch this class of bug (visual inspection alone won't obviously reveal that the wrong CSS layer supplied the color).

## Blazor Interactivity — Must Be Explicitly Enabled

The `blazor` scaffolding template (used without `--all-interactive`) renders pages as **static SSR by default** — component lifecycle methods (`OnInitializedAsync`, etc.) run and populate data fine, but nothing is interactive: clicks, tab switches, button handlers all silently do nothing, because no SignalR circuit is wired up. This looks exactly like "only the first thing rendered works, everything else is dead" — found this the hard way when only the first tab of a `MudTabs` page showed data and clicking other tabs did nothing.

**Fix already applied:** `Components/App.razor` sets `<Routes @rendermode="InteractiveServer" />`, making the whole app interactive from the root. If a new top-level component (e.g. a second `App`-style host, or anything bypassing `Routes`) doesn't inherit this, it needs its own `@rendermode="InteractiveServer"` — don't assume interactivity "just works" anywhere in this project without checking.

## Local Dev Database

- Connection string lives in **`dotnet user-secrets`**, not in `appsettings.json` (which has an empty placeholder) — this was a real finding against the legacy app (plaintext credentials committed in `environment.properties`) that must not be repeated here.
- Set it with: `dotnet user-secrets set "ConnectionStrings:eKYC" "Server=...;Database=eKYC;User Id=...;Password=...;TrustServerCertificate=True;" --project src/eKYC.Api/eKYC.Api.csproj`
- The integration test project (`eKYC.IntegrationTests`) has its own `user-secrets` store with the same connection string — set it there too if it's missing (`--project tests/eKYC.IntegrationTests/eKYC.IntegrationTests.csproj`).
- `eKYC.IntegrationTests` hits the **real** dev database on purpose — it's how column-mapping mistakes that only surface at runtime get caught. Every test that inserts data cleans up after itself (delete in a `finally` block) so repeated runs don't leave junk in a shared dev DB.

---

## Common Commands

```bash
# Build solution
dotnet build eKYC.slnx

# Run all tests (unit + integration against the real dev DB)
dotnet test eKYC.slnx

# Apply pending DbUp migrations (does not start the web host)
dotnet run --project src/eKYC.Api -- --migrate

# Run the API locally
dotnet run --project src/eKYC.Api

# Run the Blazor frontend locally
dotnet run --project src/eKYC.Web.Blazor

# Run the React frontend locally (needs the API running; proxies /api to :5124)
cd src/eKYC.Web.React && npm install && npm run dev

# Add a NuGet package to a specific project
dotnet add src/eKYC.DataAccess/eKYC.DataAccess.csproj package <PackageName>
```

Note: this solution uses the newer **`.slnx`** format (`dotnet new sln` defaults to it on .NET 10), not classic `.sln`.

---

## Task Workflow

1. Check this file and the migration-decisions artifact for context before starting.
2. Work in order: Domain → DataAccess → Application → Api → Web (Blazor first, React as a fast-follow slice).
3. After each file change, verify it compiles (`dotnet build`).
4. Write integration tests for new repository methods (they catch real Dapper/column mistakes) and unit tests for service-layer logic (mock the repository).
5. Don't add SQL outside `eKYC.DataAccess` repository classes.
6. Don't add business logic to controllers or Blazor components.
7. Don't hash passwords into a column sized for the legacy scheme — `A_Usr.Pwd` is `varchar(20)` in the existing schema and cannot hold a real hash; the Users table needs a wider column when auth/user-management work starts.

---

## What NOT to Do

- ❌ Do not use Entity Framework Core — Dapper only.
- ❌ Do not write raw SQL outside of `eKYC.DataAccess`.
- ❌ Do not put business logic in controllers or Blazor pages.
- ❌ Do not commit connection strings, passwords, or `appsettings.Production.json`.
- ❌ Do not hard-delete records — legacy convention is soft status flags (`Clnt_St`, `Usr_St`, etc.); confirm the exact column before assuming `IsActive`/`IsDeleted` naming, since eKYC's schema doesn't follow that convention consistently.
- ❌ Do not build the SafeWatch integration without checking with the user first — it was explicitly descoped.
- ❌ Do not query `HBORIS` directly from new code outside `IClientMasterDataProvider`'s interim implementation — everything else should depend on the interface, not the linked server, so the eventual API swap doesn't ripple through the codebase.
- ❌ Do not assume a table has a working FOREIGN KEY or PRIMARY KEY — verify against the live schema first (see Data Access Rules above).

## API JSON Naming (changed 2026-10-08)

`eKYC.Api` serializes JSON with **no naming policy** — property names are exactly the C# names (`Program.cs`:
`PropertyNamingPolicy = null` for MVC and for `ConfigureHttpJsonOptions`). So `A_*`/`CL_*` models go over the wire with
column names (`Clnt_Id`, `HBOR_ID`, `row_version`) and the others in PascalCase (`ClntId`). The React types in
`src/eKYC.Web.React/src/models` mirror this 1:1 — update them when a backend model changes. Blazor is unaffected
(its `HttpClient` JSON reading is case-insensitive).

## Local-Only Files (not in git)

- `New Microsoft Word Document.docx` (repo root) is the technical migration analysis of HBOR's legacy eKYC system
  (stack read, live-schema audit, the migration decisions above). It is client-internal, so it is **git-ignored on purpose** —
  keep it local, never commit or publish it, and read it for the full narrative behind the decisions in this file.
- `.env` (DB credentials for the remote server) is git-ignored too; `.env.example` is the committed template.
