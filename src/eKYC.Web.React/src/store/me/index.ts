import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { CurrentUser } from '../../models';

export type MeState = {
  me: CurrentUser | null;
  pending: boolean;
  error: string | null;
};

/** GET api/me - the caller with roles and permitted objects (tabs). */
export const getMe = createAsyncThunk<CurrentUser, void, { rejectValue: ApiError }>(
  'me/get',
  async (_, { rejectWithValue }) => {
    try {
      return (await http.get<CurrentUser>('/api/me')).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
  {
    // Several components ask for the user at once on first render; only one request goes out.
    condition: (_arg, { getState }) => !(getState() as { me: MeState }).me.pending,
  },
);

const initialState: MeState = { me: null, pending: false, error: null };

export const meSlice = createSlice({
  name: 'me',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getMe.pending, (state) => {
        state.pending = true;
        state.error = null;
      })
      .addCase(getMe.fulfilled, (state, { payload }) => {
        state.pending = false;
        state.me = payload;
      })
      .addCase(getMe.rejected, (state, { payload, error }) => {
        state.pending = false;
        state.error = payload?.message ?? error.message ?? 'Request failed';
      });
  },
});

export const meReducer = meSlice.reducer;
