-- Adds the two additive columns agreed for the .NET rewrite (schema otherwise kept as-is):
--   row_version  Pattern 1 optimistic-concurrency token (see CL_Clnt update in eKYC.DataAccess.Clients.ClientRepository)
--   tenant_id    multi-tenant discriminator, single tenant in use today
-- Other concurrency-controlled tables get the same two columns via later numbered migrations as
-- their aggregates are built out (ownership tables, etc.) — not all at once here.

ALTER TABLE CL_Clnt ADD
    row_version INT NOT NULL CONSTRAINT DF_CL_Clnt_row_version DEFAULT (1),
    tenant_id   INT NOT NULL CONSTRAINT DF_CL_Clnt_tenant_id   DEFAULT (1);
