import { ISODateString } from '../common/ISODateString';

/** Query filter for GET api/reports/risk. */
export type ReportRiskFilter = {
  AddDtFrom?: ISODateString | null;
  AddDtTo?: ISODateString | null;
  MdfDtFrom?: ISODateString | null;
  MdfDtTo?: ISODateString | null;
  ClntTypCds?: string[] | null;
  RiskEstIds?: number[] | null;
};
