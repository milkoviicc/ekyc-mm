import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { ReportKey, ReportRiskFilter, ReportRiskRow } from '../../models';
import { toReportRiskQuery } from '../../utils/queryParams';

export type ReportsState = {
  rowsByReport: Partial<Record<ReportKey, ReportRiskRow[]>>;
  pendingReports: ReportKey[];
  error: string | null;
};

/** GET api/reports/risk?reportKey=... */
export const getRiskReport = createAsyncThunk<
  { reportKey: ReportKey; rows: ReportRiskRow[] },
  { reportKey: ReportKey; filter?: ReportRiskFilter },
  { rejectValue: ApiError }
>('reports/getRisk', async ({ reportKey, filter }, { rejectWithValue }) => {
  try {
    const response = await http.get<ReportRiskRow[]>('/api/reports/risk', {
      params: { ...toReportRiskQuery(filter), reportKey },
    });
    return { reportKey, rows: response.data };
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

const initialState: ReportsState = { rowsByReport: {}, pendingReports: [], error: null };

export const reportsSlice = createSlice({
  name: 'reports',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getRiskReport.pending, (state, action) => {
        state.pendingReports.push(action.meta.arg.reportKey);
        state.error = null;
      })
      .addCase(getRiskReport.fulfilled, (state, { payload, meta }) => {
        state.pendingReports = state.pendingReports.filter((r) => r !== meta.arg.reportKey);
        state.rowsByReport[payload.reportKey] = payload.rows;
      })
      .addCase(getRiskReport.rejected, (state, { payload, error, meta }) => {
        state.pendingReports = state.pendingReports.filter((r) => r !== meta.arg.reportKey);
        state.error = payload?.message ?? error.message ?? 'Request failed';
      });
  },
});

export const reportsReducer = reportsSlice.reducer;
