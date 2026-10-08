import { A_Object_Locks } from './A_Object_Locks';

/** Body of POST api/locks - identifies the record the user is about to edit. */
export type AcquireLockRequest = {
  Object_Id: number;
  /** Record kind, e.g. "CL_Doc_Revisions" or "CL_Clnt". Together with Object_Id it identifies the record. */
  Object_Class: string;
  /** Human label shown to others, e.g. the revision subject. */
  Object_Name?: string | null;
};

/** GET api/locks/settings - lock timings. */
export type ObjectLockSettings = {
  /** Minutes without a refresh after which a lock counts as abandoned and can be taken over. */
  TtlMinutes: number;
  /** How often an open editor should refresh its lock. */
  HeartbeatSeconds: number;
};

/** Convenience alias: a successful acquire returns the lock itself. */
export type AcquiredLock = A_Object_Locks;
