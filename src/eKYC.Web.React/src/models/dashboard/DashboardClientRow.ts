import { ISODateString } from '../common/ISODateString';

/** One row of the dashboard work queue / client analysis grids (multi-table join, not a single table). */
export type DashboardClientRow = {
  ClntId: number;
  HborId: number;
  ClntTypCd: string;
  /** Client type display name, e.g. "Fizička osoba". */
  VrstaKlijenta: string;
  ClntNm: string;
  ClntSt: string;
  ClntPrcsngSt: string;
  /** Processing status display name. */
  Status?: string | null;
  RskEstId?: number | null;
  RskPnts?: number | null;
  PepInd?: string | null;
  WtchLstInd: string;
  Oib?: string | null;
  ModifiedByName?: string | null;
  MdfDt: ISODateString;
  /** Master-data (HBOR feed) differs from the local record. */
  MpChange: boolean;
  DoNotCheck: boolean;
  /** Row needs modification (drives the dark-red row color). */
  MustModify: boolean;
};
