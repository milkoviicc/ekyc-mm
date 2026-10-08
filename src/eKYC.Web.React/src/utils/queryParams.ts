import { DashboardFilter, ReportRiskFilter } from '../models';

const blankToUndefined = (value: string | null | undefined): string | undefined =>
  value && value.trim().length > 0 ? value.trim() : undefined;

/**
 * The filter models use the C# property names (PascalCase), the API query-string parameters are camelCase.
 * Blank / null values are dropped so they are not sent at all (axios skips undefined).
 */
export function toDashboardQuery(filter?: DashboardFilter) {
  return {
    clntTypCd: blankToUndefined(filter?.ClntTypCd),
    clntNm: blankToUndefined(filter?.ClntNm),
    clntPrcsngSt: blankToUndefined(filter?.ClntPrcsngSt),
    oib: blankToUndefined(filter?.Oib),
    rskEstId: filter?.RskEstId ?? undefined,
  };
}

export function toReportRiskQuery(filter?: ReportRiskFilter) {
  return {
    addDtFrom: filter?.AddDtFrom ?? undefined,
    addDtTo: filter?.AddDtTo ?? undefined,
    mdfDtFrom: filter?.MdfDtFrom ?? undefined,
    mdfDtTo: filter?.MdfDtTo ?? undefined,
    clntTypCds: filter?.ClntTypCds && filter.ClntTypCds.length > 0 ? filter.ClntTypCds : undefined,
    riskEstIds: filter?.RiskEstIds && filter.RiskEstIds.length > 0 ? filter.RiskEstIds : undefined,
  };
}
