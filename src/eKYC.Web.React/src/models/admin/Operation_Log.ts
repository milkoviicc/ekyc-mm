import { ISODateString } from '../common/ISODateString';

/** Audit trail row - table Operation_Log - joined to A_Usr for the actor's name. */
export type Operation_Log = {
  Log_Id: number;
  Log_Timestamp: ISODateString;
  User_Id: number;
  Object_Type: string;
  Object_Id: number;
  Object_Code: string;
  Operation_Type: string;
  Log_Remark: string;
  /** Joined from A_Usr; null if the user no longer exists. */
  Lgn_Nm?: string | null;
  Usr_Nm_Fst?: string | null;
  Usr_Nm_Lst?: string | null;
};

/** Query filter for GET api/admin/audit (OperationLogFilter on the backend). All fields optional. */
export type OperationLogFilter = {
  From?: ISODateString | null;
  To?: ISODateString | null;
  User_Id?: number | null;
  Object_Type?: string | null;
  Take?: number;
};
