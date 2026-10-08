import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { Operation_Log, OperationLogFilter } from '../../models';

export type AuditState = {
  rows: Operation_Log[] | null;
  objectTypes: string[] | null;
  pending: boolean;
  error: string | null;
};

/** GET api/admin/audit - newest first, at most Take rows (default 500, server cap 5000). */
export const getAudit = createAsyncThunk<Operation_Log[], OperationLogFilter | undefined, { rejectValue: ApiError }>(
  'audit/get',
  async (filter, { rejectWithValue }) => {
    try {
      const params = {
        from: filter?.From ?? undefined,
        to: filter?.To ?? undefined,
        userId: filter?.User_Id ?? undefined,
        objectType: filter?.Object_Type || undefined,
        take: filter?.Take ?? undefined,
      };
      return (await http.get<Operation_Log[]>('/api/admin/audit', { params })).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** GET api/admin/audit/object-types - distinct object types for the filter dropdown. */
export const getAuditObjectTypes = createAsyncThunk<string[], void, { rejectValue: ApiError }>(
  'audit/objectTypes',
  async (_, { rejectWithValue }) => {
    try {
      return (await http.get<string[]>('/api/admin/audit/object-types')).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

const initialState: AuditState = { rows: null, objectTypes: null, pending: false, error: null };

export const auditSlice = createSlice({
  name: 'audit',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getAudit.pending, (state) => {
        state.pending = true;
        state.error = null;
      })
      .addCase(getAudit.fulfilled, (state, { payload }) => {
        state.pending = false;
        state.rows = payload;
      })
      .addCase(getAudit.rejected, (state, { payload, error }) => {
        state.pending = false;
        state.error = payload?.message ?? error.message ?? 'Request failed';
      })
      .addCase(getAuditObjectTypes.fulfilled, (state, { payload }) => {
        state.objectTypes = payload;
      });
  },
});

export const auditReducer = auditSlice.reducer;
