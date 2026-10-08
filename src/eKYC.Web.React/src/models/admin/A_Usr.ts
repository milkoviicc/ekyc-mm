import { ISODateString } from '../common/ISODateString';

/** Application user - table A_Usr. The legacy Pwd column is never returned by the API. */
export type A_Usr = {
  Usr_Id: number;
  Lgn_Nm?: string | null;
  Usr_Nm_Fst?: string | null;
  Usr_Nm_Lst?: string | null;
  Pwd_Rqd_Ind?: boolean | null;
  Pwd_St?: string | null;
  Pwd_Dt?: ISODateString | null;
  Pwd_Life?: number | null;
  Org_Id?: number | null;
  /** Status flag (char(1)) - soft-delete convention, never hard-deleted. */
  Usr_St?: string | null;
  Prsn_Id?: number | null;
  /** Legacy single-role column; multiple roles live in A_Usr_ISRole. */
  ISRol_Id?: number | null;
  Email?: string | null;
  Lst_Lgn_Dt?: ISODateString | null;
  Lgn_Try_Cnt?: number | null;
  Add_By?: number | null;
  Add_Dt?: ISODateString | null;
  Mdf_By?: number | null;
  Mdf_Dt?: ISODateString | null;
  Logged: boolean;
  Current_Host: string;
  SessionId?: string | null;
};
