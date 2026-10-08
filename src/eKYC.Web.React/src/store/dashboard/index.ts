import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { DashboardClientRow, DashboardFilter } from '../../models';
import { toDashboardQuery } from '../../utils/queryParams';

export type DashboardState = {
  rows: DashboardClientRow[] | null;
  pending: boolean;
  error: string | null;
};

/** GET api/dashboard/work-queue - clients awaiting processing, filtered. */
export const getWorkQueue = createAsyncThunk<DashboardClientRow[], DashboardFilter | undefined, { rejectValue: ApiError }>(
  'dashboard/getWorkQueue',
  async (filter, { rejectWithValue }) => {
    try {
      const response = await http.get<DashboardClientRow[]>('/api/dashboard/work-queue', {
        params: toDashboardQuery(filter),
      });
      return response.data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

const initialState: DashboardState = { rows: null, pending: false, error: null };

export const dashboardSlice = createSlice({
  name: 'dashboard',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getWorkQueue.pending, (state) => {
        state.pending = true;
        state.error = null;
        state.rows = null;
      })
      .addCase(getWorkQueue.fulfilled, (state, { payload }) => {
        state.pending = false;
        state.rows = payload;
      })
      .addCase(getWorkQueue.rejected, (state, { payload, error }) => {
        state.pending = false;
        state.error = payload?.message ?? error.message ?? 'Request failed';
      });
  },
});

export const dashboardReducer = dashboardSlice.reducer;
