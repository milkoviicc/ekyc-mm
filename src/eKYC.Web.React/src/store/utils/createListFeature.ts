import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';

export type ListState<T> = {
  items: T[] | null;
  pending: boolean;
  error: string | null;
};

/**
 * Builds the thunk + reducer for a plain "GET a whole list" resource (all the read-only lookup and admin endpoints).
 * Features with writes (clients, revisions, locks) have their own actions.ts/reducer.ts instead.
 */
export function createListFeature<T>(name: string, endpoint: string) {
  const fetchAll = createAsyncThunk<T[], void, { rejectValue: ApiError }>(
    `${name}/fetchAll`,
    async (_, { rejectWithValue }) => {
      try {
        const response = await http.get<T[]>(endpoint);
        return response.data;
      } catch (error) {
        return rejectWithValue(toApiError(error));
      }
    },
  );

  const initialState: ListState<T> = { items: null, pending: false, error: null };

  const slice = createSlice({
    name,
    initialState,
    reducers: {},
    extraReducers: (builder) => {
      builder
        .addCase(fetchAll.pending, (state) => {
          (state as ListState<T>).pending = true;
          (state as ListState<T>).error = null;
        })
        .addCase(fetchAll.fulfilled, (state, action) => {
          (state as ListState<T>).pending = false;
          (state as ListState<T>).items = action.payload;
        })
        .addCase(fetchAll.rejected, (state, action) => {
          (state as ListState<T>).pending = false;
          (state as ListState<T>).error = action.payload?.message ?? action.error.message ?? 'Request failed';
        });
    },
  });

  return { fetchAll, reducer: slice.reducer };
}
