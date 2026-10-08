/** Valid values of the reportKey query parameter of GET api/reports/risk. */
export const ReportKeys = {
  RiskPeriod: 'risk-period',
  RiskDay: 'risk-day',
  HighRiskReprocess: 'high-risk-reprocess',
} as const;

export type ReportKey = (typeof ReportKeys)[keyof typeof ReportKeys];
