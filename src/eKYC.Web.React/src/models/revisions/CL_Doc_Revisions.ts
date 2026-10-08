import { ISODateString } from '../common/ISODateString';

/** Document / audit revision record ("Revizija" tab) - table CL_Doc_Revisions. */
export type CL_Doc_Revisions = {
  CL_Doc_Revision_Id: number;
  /** Despite the name, holds a RevisionType.RevTypeId (CL_Rev_Typ) value - faithful to the legacy schema. */
  CL_Doc_Typ_Rev_Id: number;
  Rev_Date: ISODateString;
  Rev_Range_From?: ISODateString | null;
  Rev_Range_To?: ISODateString | null;
  Rev_Done_By: string;
  Subject: string;
  Recommendation?: string | null;
  row_version: number;
  tenant_id: number;
};
