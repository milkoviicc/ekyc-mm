/** Role-to-object permission / menu entry, including its menu hierarchy - table A_ISRole_Objct. */
export type A_ISRole_Objct = {
  ISRol_Objct_Id: number;
  ISRol_Id?: number | null;
  Objct_Id?: number | null;
  Objct_Ttl?: string | null;
  Prnt_Objct_id?: number | null;
  Sqnc_No?: number | null;
  /** Access modifier code (char(2)). */
  Mdfr_Cd?: string | null;
  Hrchy_Lvl?: number | null;
  Hrchy_Path?: string | null;
};
