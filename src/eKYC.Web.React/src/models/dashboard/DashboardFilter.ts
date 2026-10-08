/** Query filter for GET api/dashboard/work-queue and api/client-analysis. All fields optional (AND-combined). */
export type DashboardFilter = {
  ClntTypCd?: string | null;
  ClntNm?: string | null;
  ClntPrcsngSt?: string | null;
  Oib?: string | null;
  RskEstId?: number | null;
};
