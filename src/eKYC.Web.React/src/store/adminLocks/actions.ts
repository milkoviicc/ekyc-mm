import { createAsyncThunk } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { A_Object_Locks } from '../../models';

const RESOURCE = '/api/admin/locks';

/** GET api/admin/locks - ADMIN role only. */
export const getObjectLocks = createAsyncThunk<A_Object_Locks[], void, { rejectValue: ApiError }>(
  'adminLocks/getAll',
  async (_, { rejectWithValue }) => {
    try {
      return (await http.get<A_Object_Locks[]>(RESOURCE)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** DELETE api/admin/locks/{id} - force-releases a lock. */
export const deleteObjectLock = createAsyncThunk<void, number, { rejectValue: ApiError }>(
  'adminLocks/delete',
  async (id, { rejectWithValue }) => {
    try {
      await http.delete(`${RESOURCE}/${id}`);
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);
