import { createAsyncThunk } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { A_Object_Locks, AcquireLockRequest, ObjectLockSettings } from '../../models';

// Pessimistic edit locks (Pattern 2) for the current user: api/locks. The A_Object_Locks admin list lives in ../adminLocks.
// These thunks keep no state in the store - useEditLock holds the lock of the open editor locally.

type Config = { rejectValue: ApiError };

/**
 * POST api/locks. Fulfilled = the caller holds the lock. Rejected with status 409 = someone else is editing and
 * `currentRecord` is their lock (holder name, age).
 */
export const acquireLock = createAsyncThunk<A_Object_Locks, AcquireLockRequest, Config>(
  'editLocks/acquire',
  async (request, { rejectWithValue }) => {
    try {
      return (await http.post<A_Object_Locks>('/api/locks', request)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** PUT api/locks/{id} - keeps the lock alive. 409 = the lock was lost. */
export const heartbeatLock = createAsyncThunk<void, number, Config>('editLocks/heartbeat', async (id, { rejectWithValue }) => {
  try {
    await http.put(`/api/locks/${id}`);
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

/** DELETE api/locks/{id} - releases the lock. */
export const releaseLock = createAsyncThunk<void, number, Config>('editLocks/release', async (id, { rejectWithValue }) => {
  try {
    await http.delete(`/api/locks/${id}`);
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

/** GET api/locks/settings - TTL and heartbeat interval. */
export const getLockSettings = createAsyncThunk<ObjectLockSettings, void, Config>('editLocks/settings', async (_, { rejectWithValue }) => {
  try {
    return (await http.get<ObjectLockSettings>('/api/locks/settings')).data;
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});
