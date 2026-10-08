import { createSlice, isAnyOf } from '@reduxjs/toolkit';
import { createRevision, deleteRevision, getRevisions, updateRevision } from './actions';
import { RevisionsState } from './revisions.dto';

const initialState: RevisionsState = {
  revisions: null,
  pending: false,
  pendingAction: false,
  error: null,
};

const writeActions = [createRevision, updateRevision, deleteRevision];

export const revisionsSlice = createSlice({
  name: 'revisions',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(getRevisions.pending, (state) => {
        state.pending = true;
        state.error = null;
      })
      .addCase(getRevisions.fulfilled, (state, { payload }) => {
        state.pending = false;
        state.revisions = payload;
      })
      .addCase(getRevisions.rejected, (state, { payload, error }) => {
        state.pending = false;
        state.error = payload?.message ?? error.message ?? 'Request failed';
      })
      .addMatcher(isAnyOf(...writeActions.map((a) => a.pending)), (state) => {
        state.pendingAction = true;
      })
      .addMatcher(isAnyOf(...writeActions.flatMap((a) => [a.fulfilled, a.rejected])), (state) => {
        state.pendingAction = false;
      });
  },
});

export const revisionsReducer = revisionsSlice.reducer;
