/** Client processing (workflow) status - table CL_Clnt_PrcsSt. */
export type ClientProcessingStatus = {
  ClntPrcsStId: number;
  ClntPrcsStCd: string;
  /** Display name. */
  Status?: string | null;
  ClntPrcsStDspn?: string | null;
};
