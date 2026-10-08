import { ISODateString } from '../common/ISODateString';

/** Held edit-session lock - table A_Object_Locks - joined to A_Usr for the holder name. */
export type A_Object_Locks = {
  Object_Lock_Id: number;
  Object_Id: number;
  Object_Class: string;
  Object_Name?: string | null;
  User_Id: number;
  Locked_At: ISODateString;
  Computer_Name?: string | null;
  /** Joined from A_Usr; null if the user no longer exists. */
  Lgn_Nm?: string | null;
  Usr_Nm_Fst?: string | null;
  Usr_Nm_Lst?: string | null;
  /** Seconds since the lock was taken or last refreshed (computed by the server clock). */
  Age_Seconds?: number | null;
};
