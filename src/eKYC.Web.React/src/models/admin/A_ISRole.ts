/** Information-system role (UNOS, ODOBR1, ODOBR2, ADMIN, ...) - table A_ISRole. */
export type A_ISRole = {
  ISRol_Id: number;
  ISRol_Nm?: string | null;
  /** Status code - joins to A_ISRole_St.ISRol_St. */
  ISRol_St?: string | null;
  Apl_Cd?: string | null;
};
