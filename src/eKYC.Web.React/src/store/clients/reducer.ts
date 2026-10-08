import { createSlice } from '@reduxjs/toolkit';
import { createClient, getClients, updateClient } from './actions';
import { ClientsState } from './clients.dto';

const initialState: ClientsState = {
  clients: null,
  pending: false,
  pendingAction: false,
  error: null,
};

export const clientsSlice = createSlice({
  name: 'clients',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getClients.pending, (state) => {
        state.pending = true;
        state.error = null;
      })
      .addCase(getClients.fulfilled, (state, { payload }) => {
        state.pending = false;
        state.clients = payload;
      })
      .addCase(getClients.rejected, (state, { payload, error }) => {
        state.pending = false;
        state.error = payload?.message ?? error.message ?? 'Request failed';
      })
      .addCase(createClient.pending, (state) => {
        state.pendingAction = true;
      })
      .addCase(createClient.fulfilled, (state) => {
        state.pendingAction = false;
      })
      .addCase(createClient.rejected, (state) => {
        state.pendingAction = false;
      })
      .addCase(updateClient.pending, (state) => {
        state.pendingAction = true;
      })
      .addCase(updateClient.fulfilled, (state) => {
        state.pendingAction = false;
      })
      .addCase(updateClient.rejected, (state) => {
        state.pendingAction = false;
      });
  },
});

export const clientsReducer = clientsSlice.reducer;
