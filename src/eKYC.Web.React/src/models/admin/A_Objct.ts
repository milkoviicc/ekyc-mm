/** Securable application object - form, menu item, ... - table A_Objct. */
export type A_Objct = {
  Objct_Id: number;
  Objct_Typ_Id?: number | null;
  Objct_Nm?: string | null;
  Objct_Dspn?: string | null;
  /** Legacy Java class/view the object invoked. */
  Objct_Call?: string | null;
  Asmbly_Cd?: string | null;
  Objct_St?: string | null;
};
