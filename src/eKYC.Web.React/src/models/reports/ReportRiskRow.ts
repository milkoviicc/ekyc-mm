import { ISODateString } from '../common/ISODateString';

/** One row of a risk report. */
export type ReportRiskRow = {
  ClntId: number;
  HborId: number;
  Oib?: string | null;
  ClntPrcsngSt: string;
  RskEstId?: number | null;
  RiskLevel?: string | null;
  RskPnts?: number | null;
  PepInd: string;
  WtchLstInd: string;
  ClntTypCd: string;
  VrstaKlijenta: string;
  ClntNm: string;
  AddDt: ISODateString;
  MdfDt: ISODateString;
};
