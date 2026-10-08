import { A_ISRole } from './A_ISRole';
import { A_Objct } from './A_Objct';
import { A_Usr } from './A_Usr';

/** Response of GET api/me - who is calling, with their roles and the application objects (tabs) they may open. */
export type CurrentUser = {
  /** Login name without domain, e.g. "Admin" for "HBOR\Admin". */
  LoginName: string;
  /** True when an active A_Usr row exists for LoginName. */
  Matched: boolean;
  User?: A_Usr | null;
  /** Roles on the authenticated principal - what the API authorizes with today. */
  Roles: string[];
  /** Roles assigned in A_Usr_ISRole that are valid today (informational until roles are read from the database). */
  DbRoles: A_ISRole[];
  /** A_Objct rows (tabs) granted to Roles through A_ISRole_Objct. Asmbly_Cd is the stable code, e.g. "tabAdmin". */
  Permissions: A_Objct[];
};
