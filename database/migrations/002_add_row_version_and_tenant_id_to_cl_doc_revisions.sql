-- Same additive pattern as migration 001, applied to the next concurrency-controlled table:
-- CL_Doc_Revisions gets real create/edit/delete in the .NET rewrite (Revizija tab), so it needs
-- Pattern 1 (optimistic locking) and the tenant discriminator, same as CL_Clnt.

ALTER TABLE CL_Doc_Revisions ADD
    row_version INT NOT NULL CONSTRAINT DF_CL_Doc_Revisions_row_version DEFAULT (1),
    tenant_id   INT NOT NULL CONSTRAINT DF_CL_Doc_Revisions_tenant_id   DEFAULT (1);
