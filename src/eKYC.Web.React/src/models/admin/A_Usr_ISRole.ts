import { ISODateString } from '../common/ISODateString';

/** User-to-role assignment with validity window - table A_Usr_ISRole. */
export type A_Usr_ISRole = {
  Usr_ISRol_Id: number;
  Usr_Id?: number | null;
  ISRol_Id?: number | null;
  Vld_From_Dt?: ISODateString | null;
  Vld_To_Dt?: ISODateString | null;
};
