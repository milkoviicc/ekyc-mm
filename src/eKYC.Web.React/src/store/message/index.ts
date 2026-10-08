import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export type MessageSeverity = 'success' | 'info' | 'warning' | 'error';

export type AppMessage = {
  id: number;
  severity: MessageSeverity;
  text: string;
};

export type MessageState = {
  queue: AppMessage[];
};

let nextId = 1;

const initialState: MessageState = { queue: [] };

/** Snackbar notifications (the React counterpart of Blazor's ISnackbar). Rendered by MessageManager. */
export const messageSlice = createSlice({
  name: 'message',
  initialState,
  reducers: {
    showMessage: {
      reducer: (state, action: PayloadAction<AppMessage>) => {
        state.queue.push(action.payload);
      },
      prepare: (severity: MessageSeverity, text: string) => ({ payload: { id: nextId++, severity, text } }),
    },
    dismissMessage: (state, action: PayloadAction<number>) => {
      state.queue = state.queue.filter((m) => m.id !== action.payload);
    },
  },
});

export const { showMessage, dismissMessage } = messageSlice.actions;
export const showSuccessMessage = (text: string) => showMessage('success', text);
export const showWarningMessage = (text: string) => showMessage('warning', text);
export const showErrorMessage = (text: string) => showMessage('error', text);

export const messageReducer = messageSlice.reducer;
