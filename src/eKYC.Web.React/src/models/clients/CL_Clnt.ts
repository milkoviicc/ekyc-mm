import { ISODateString } from '../common/ISODateString';

/** A bank client under AML/KYC review (individual, legal entity or correspondent bank) - table CL_Clnt. */
export type CL_Clnt = {
  Clnt_Id: number;
  /** Foreign key into the upstream HBOR client-master source, not a local identity. */
  HBOR_ID: number;
  Clnt_Typ_Cd: string;
  Clnt_St: string;
  /** Workflow status, stored as the CL_Clnt_PrcsSt.Clnt_PrcsSt_Cd string. */
  Clnt_Prcsng_St: string;
  RskEst_Id?: number | null;
  Rsk_Pnts?: number | null;
  PEP_Ind?: string | null;
  Rmrk?: string | null;
  WtchLst_Ind: string;
  Add_By: number;
  Add_Dt: ISODateString;
  Mdf_By: number;
  Mdf_Dt: ISODateString;
  /** Optimistic-concurrency token (Pattern 1). Send back unchanged on update. */
  row_version: number;
  /** Multi-tenant discriminator; the server overwrites it on write. */
  tenant_id: number;
};
