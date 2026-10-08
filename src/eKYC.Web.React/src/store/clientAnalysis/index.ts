import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { DashboardClientRow, DashboardFilter } from '../../models';
import { toDashboardQuery } from '../../utils/queryParams';

export type ClientAnalysisState = {
  /** Rows per client type code (P, L, BEU, BINT); undefined = not loaded yet for the current filter. */
  rowsByType: Record<string, DashboardClientRow[] | undefined>;
  pendingTypes: string[];
  error: string | null;
};

/** GET api/client-analysis?clntTypCd=... - all clients of one type, including active/closed/rejected. */
export const getClientAnalysis = createAsyncThunk<
  { clntTypCd: string; rows: DashboardClientRow[] },
  { clntTypCd: string; filter?: DashboardFilter },
  { rejectValue: ApiError }
>('clientAnalysis/getByType', async ({ clntTypCd, filter }, { rejectWithValue }) => {
  try {
    const response = await http.get<DashboardClientRow[]>('/api/client-analysis', {
      // clntTypCd is the tab, so it always overrides whatever the filter carries.
      params: { ...toDashboardQuery(filter), clntTypCd },
    });
    return { clntTypCd, rows: response.data };
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

const initialState: ClientAnalysisState = { rowsByType: {}, pendingTypes: [], error: null };

export const clientAnalysisSlice = createSlice({
  name: 'clientAnalysis',
  initialState,
  reducers: {
    /** Called when the filter changes - cached tabs no longer match it. */
    resetClientAnalysis: (state) => {
      state.rowsByType = {};
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(getClientAnalysis.pending, (state, action) => {
        state.pendingTypes.push(action.meta.arg.clntTypCd);
        state.error = null;
      })
      .addCase(getClientAnalysis.fulfilled, (state, { payload, meta }) => {
        state.pendingTypes = state.pendingTypes.filter((t) => t !== meta.arg.clntTypCd);
        state.rowsByType[payload.clntTypCd] = payload.rows;
      })
      .addCase(getClientAnalysis.rejected, (state, { payload, error, meta }) => {
        state.pendingTypes = state.pendingTypes.filter((t) => t !== meta.arg.clntTypCd);
        state.error = payload?.message ?? error.message ?? 'Request failed';
      });
  },
});

export const { resetClientAnalysis } = clientAnalysisSlice.actions;
export const clientAnalysisReducer = clientAnalysisSlice.reducer;
