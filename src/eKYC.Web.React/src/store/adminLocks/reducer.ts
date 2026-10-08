import { createSlice } from '@reduxjs/toolkit';
import { A_Object_Locks } from '../../models';
import { deleteObjectLock, getObjectLocks } from './actions';

export type AdminLocksState = {
  locks: A_Object_Locks[] | null;
  pending: boolean;
  pendingAction: boolean;
  error: string | null;
};

const initialState: AdminLocksState = {
  locks: null,
  pending: false,
  pendingAction: false,
  error: null,
};

export const adminLocksSlice = createSlice({
  name: 'adminLocks',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getObjectLocks.pending, (state) => {
        state.pending = true;
        state.error = null;
      })
      .addCase(getObjectLocks.fulfilled, (state, { payload }) => {
        state.pending = false;
        state.locks = payload;
      })
      .addCase(getObjectLocks.rejected, (state, { payload, error }) => {
        state.pending = false;
        state.error = payload?.message ?? error.message ?? 'Request failed';
      })
      .addCase(deleteObjectLock.pending, (state) => {
        state.pendingAction = true;
      })
      .addCase(deleteObjectLock.fulfilled, (state) => {
        state.pendingAction = false;
      })
      .addCase(deleteObjectLock.rejected, (state) => {
        state.pendingAction = false;
      });
  },
});

export const adminLocksReducer = adminLocksSlice.reducer;
