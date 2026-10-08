# eKYC.Web.React

React + TypeScript front end for eKYC (comparison build next to the Blazor app). Vite, Redux Toolkit, Material UI.
It only talks to `eKYC.Api` - never to the database.

## Run

```bash
# 1. API (http://localhost:5124)
dotnet run --project src/eKYC.Api

# 2. React (http://localhost:3000) - /api is proxied to the API, so no CORS setup is needed
cd src/eKYC.Web.React
npm install
npm run dev
```

Other API port: `VITE_API_PROXY_TARGET=http://localhost:5199 npm run dev`.
Production build: `npm run build` (type-checks first, output in `build/`); set `VITE_API_BASE_URL` in `.env.production`.
Type check only: `npm run typecheck`.

## Structure (mirrors the Hidrostres web project)

```
src/
  index.tsx, App.tsx          entry, providers (Redux, Router, MUI theme, date pickers), routes
  http-common.ts              axios instance + ApiError / 409 helpers
  config/                     (reserved for constants)
  models/<area>/*.ts          one TypeScript type per backend model - see below
  store/<feature>/            Redux Toolkit: actions.ts + reducer.ts + *.dto.ts + index.ts
  store/utils/                createListFeature (thunk + reducer for plain "GET list" endpoints)
  hooks/                      useReferenceData (loads lookups once), usePermissions (api/me), useEditLock (pessimistic lock), useUrlTab
  modules/layout/             Layout (app bar + drawer), Sidebar
  modules/shared/             AppDataGrid, SelectField, ConfirmDialog, MessageManager, PageHeader, ...
  modules/sections/<screen>/  one folder per screen
  utils/                      routes, formatting, query-param mapping, dashboard row colors
  styles/theme.ts             MUI theme (Croatian locale)
```

## Design system

- **Theme**: all colors, radius and typography live in `styles/theme.ts` (Inter variable font, 12px radius, soft borders instead of shadows). Light and dark mode: follows the OS, the toggle in the top bar remembers the choice (`styles/ColorModeProvider.tsx`).
- **Navigation**: grouped sidebar (Rad / Pregled / Sustav) that collapses to an icon rail; breadcrumbs on every page; the selected tab of Izvješća, Administriranje and Klijenti is kept in the URL (`?tab=`), so tabs can be bookmarked and the back button works.
- **Lists**: `AppDataGrid` (toolbar with search, column picker, filters, density, CSV export), status and risk shown as colored chips (`utils/statusColors.ts`), collapsible `FilterBar` that shows applied filters as removable chips, skeleton loading and friendly empty states.
- **Dashboard**: KPI tiles that double as quick filters, rows tinted by meaning (red = needs change / high-risk change, amber = rejected, green = active) instead of saturated colors, details side panel on row click.
- **Accessibility**: skip link, visible focus ring, real buttons with `aria-pressed` for toggles, `prefers-reduced-motion` respected, status never conveyed by color alone (chip text + legend).

## Types match the API one-to-one

`eKYC.Api` serializes JSON with the **C# property names unchanged** (no camelCase, see `Program.cs`), and the
`A_*` / `CL_*` models use the table's column names. So `models/clients/CL_Clnt.ts` has `Clnt_Id`, `HBOR_ID`,
`row_version`, ... exactly as in SQL Server. Dates are ISO strings (`ISODateString`).

| Folder | Types |
|---|---|
| `models/admin` | `A_Usr`, `A_ISRole`, `A_ISRole_St`, `A_Usr_ISRole`, `A_ISRole_Objct`, `A_Objct`, `A_Objct_Typs`, `A_Apl_Prmtr`, `A_Object_Locks` |
| `models/clients` | `CL_Clnt` |
| `models/revisions` | `CL_Doc_Revisions`, `DocRevisionFilter`, `RevisionType` |
| `models/dashboard` | `DashboardClientRow`, `DashboardFilter` |
| `models/referenceData` | `ClState`, `ClientType`, `ClientProcessingStatus`, `ClientProcessingStatusTransition`, `DocumentType`, `OwnershipType`, `RiskClass`, `RiskEstimate` |
| `models/reports` | `ReportKeys`, `ReportRiskFilter`, `ReportRiskRow` |
| `models/common` | `ISODateString`, `ConcurrencyConflict<T>` |

When a backend model changes, update the matching type here.

## Screens (same as the Blazor app)

| Route | Screen |
|---|---|
| `/` | Nadzorna ploča (dashboard work queue, colored rows) |
| `/klijenti-analiza`, `/klijenti-pregled` | Clients by type (4 tabs), filterable |
| `/izvjesca` | Risk reports (3 tabs) |
| `/revizija` | Revisions: filter, create, edit, delete |
| `/administriranje` | Prava pristupa (users + roles, role x screen matrix, screens with their roles), Postavke (parameters), Zaključavanje (locks), Dnevnik promjena (audit trail) |
| `/clients` | CL_Clnt grid + edit dialog (optimistic locking) |
| `/reference-data` | All lookup tables |

## Concurrency (row_version)

Edit dialogs send the `row_version` they loaded. On HTTP 409 the thunk rejects with `ApiError.status === 409`
(`currentRecord` carries the fresh row); the screens show a warning and reload.

## Auth

No login screen. In Development the API signs in a dev user (UNOS + ADMIN roles). On IIS the API uses Windows
(Negotiate) auth; `withCredentials` is already on for that.
