import { configureStore } from '@reduxjs/toolkit';
import { adminReducer } from './admin';
import { adminLocksReducer } from './adminLocks';
import { clientAnalysisReducer } from './clientAnalysis';
import { clientsReducer } from './clients';
import { dashboardReducer } from './dashboard';
import { auditReducer } from './audit';
import { meReducer } from './me';
import { messageReducer } from './message';
import { referenceDataReducer } from './referenceData';
import { reportsReducer } from './reports';
import { revisionsReducer } from './revisions';

const store = configureStore({
  reducer: {
    message: messageReducer,
    me: meReducer,
    audit: auditReducer,
    referenceData: referenceDataReducer,
    dashboard: dashboardReducer,
    clientAnalysis: clientAnalysisReducer,
    clients: clientsReducer,
    reports: reportsReducer,
    revisions: revisionsReducer,
    admin: adminReducer,
    adminLocks: adminLocksReducer,
  },
});

export default store;

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
