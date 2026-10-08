/** Risk estimate band - table CL_Rsk_Est. */
export type RiskEstimate = {
  RskEstId: number;
  LowerPoints?: number | null;
  UpperPoints?: number | null;
  RiskLevel?: string | null;
  AnalysisType?: string | null;
};
