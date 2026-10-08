import { createAsyncThunk } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { A_Apl_Prmtr, A_ISRole_Objct, A_Usr, A_Usr_ISRole } from '../../models';

// Write endpoints under api/admin/* (ADMIN role). Lists are re-read by the screens after a successful write.
// Every call is audited on the server (Operation_Log). Users and parameters have no row_version: the last save wins.

type Config = { rejectValue: ApiError };

/** POST api/admin/users - login name is required, unique and max 20 characters. */
export const createUser = createAsyncThunk<A_Usr, Partial<A_Usr>, Config>('admin/createUser', async (user, { rejectWithValue }) => {
  try {
    return (await http.post<A_Usr>('/api/admin/users', user)).data;
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

/** PUT api/admin/users/{id} - name, login, e-mail and status (deactivate with Usr_St = 'I'; users are never deleted). */
export const updateUser = createAsyncThunk<A_Usr, A_Usr, Config>('admin/updateUser', async (user, { rejectWithValue }) => {
  try {
    return (await http.put<A_Usr>(`/api/admin/users/${user.Usr_Id}`, user)).data;
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

/** POST api/admin/user-roles - gives a user a role for a period. */
export const assignRole = createAsyncThunk<A_Usr_ISRole, Pick<A_Usr_ISRole, 'Usr_Id' | 'ISRol_Id' | 'Vld_From_Dt' | 'Vld_To_Dt'>, Config>(
  'admin/assignRole',
  async (assignment, { rejectWithValue }) => {
    try {
      return (await http.post<A_Usr_ISRole>('/api/admin/user-roles', assignment)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** POST api/admin/user-roles/{id}/revoke - ends the assignment now (the row is kept for history). */
export const revokeRole = createAsyncThunk<void, number, Config>('admin/revokeRole', async (id, { rejectWithValue }) => {
  try {
    await http.post(`/api/admin/user-roles/${id}/revoke`);
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

/** POST api/admin/role-objects - lets a role open an object (tab). */
export const grantPermission = createAsyncThunk<A_ISRole_Objct, { ISRol_Id: number; Objct_Id: number }, Config>(
  'admin/grantPermission',
  async (grant, { rejectWithValue }) => {
    try {
      return (await http.post<A_ISRole_Objct>('/api/admin/role-objects', grant)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** DELETE api/admin/role-objects/{id} - takes a right away from a role. */
export const revokePermission = createAsyncThunk<void, number, Config>('admin/revokePermission', async (id, { rejectWithValue }) => {
  try {
    await http.delete(`/api/admin/role-objects/${id}`);
  } catch (error) {
    return rejectWithValue(toApiError(error));
  }
});

/** POST api/admin/parameters - code is unique and immutable afterwards. */
export const createParameter = createAsyncThunk<A_Apl_Prmtr, Pick<A_Apl_Prmtr, 'Prmtr_Cd' | 'Prmtr_Val' | 'Prmtr_Dspn'>, Config>(
  'admin/createParameter',
  async (parameter, { rejectWithValue }) => {
    try {
      return (await http.post<A_Apl_Prmtr>('/api/admin/parameters', parameter)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** PUT api/admin/parameters/{id} - value and description (last save wins). */
export const updateParameter = createAsyncThunk<A_Apl_Prmtr, A_Apl_Prmtr, Config>(
  'admin/updateParameter',
  async (parameter, { rejectWithValue }) => {
    try {
      return (await http.put<A_Apl_Prmtr>(`/api/admin/parameters/${parameter.Apl_Id}`, parameter)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);
