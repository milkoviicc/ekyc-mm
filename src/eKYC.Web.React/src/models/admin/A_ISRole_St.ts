/**
 * Role status lookup - table A_ISRole_St. The table has no primary key and the API exposes no endpoint for it yet;
 * the type exists so it is ready when one is added.
 */
export type A_ISRole_St = {
  ISRol_St?: string | null;
  Stts_Nm?: string | null;
};
