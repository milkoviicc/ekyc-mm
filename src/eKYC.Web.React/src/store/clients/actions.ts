import { createAsyncThunk } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { CL_Clnt } from '../../models';

const RESOURCE = '/api/clients';

export const getClients = createAsyncThunk<CL_Clnt[], void, { rejectValue: ApiError }>(
  'clients/getAll',
  async (_, { rejectWithValue }) => {
    try {
      return (await http.get<CL_Clnt[]>(RESOURCE)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

export const getClientById = createAsyncThunk<CL_Clnt, number, { rejectValue: ApiError }>(
  'clients/getById',
  async (id, { rejectWithValue }) => {
    try {
      return (await http.get<CL_Clnt>(`${RESOURCE}/${id}`)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** POST api/clients - needs the UNOS or ADMIN role. The server assigns the id, row_version and tenant. */
export const createClient = createAsyncThunk<CL_Clnt, CL_Clnt, { rejectValue: ApiError }>(
  'clients/create',
  async (client, { rejectWithValue }) => {
    try {
      return (await http.post<CL_Clnt>(RESOURCE, client)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/**
 * PUT api/clients/{id} - the submitted row_version must match the stored one, otherwise the API answers 409 and the
 * rejected ApiError carries the freshly reloaded record in `currentRecord`.
 */
export const updateClient = createAsyncThunk<void, CL_Clnt, { rejectValue: ApiError }>(
  'clients/update',
  async (client, { rejectWithValue }) => {
    try {
      await http.put(`${RESOURCE}/${client.Clnt_Id}`, client);
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);
