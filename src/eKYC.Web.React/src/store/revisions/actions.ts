import { createAsyncThunk } from '@reduxjs/toolkit';
import http, { ApiError, toApiError } from '../../http-common';
import { CL_Doc_Revisions, DocRevisionFilter } from '../../models';

const RESOURCE = '/api/revisions';

export const getRevisions = createAsyncThunk<CL_Doc_Revisions[], DocRevisionFilter | undefined, { rejectValue: ApiError }>(
  'revisions/getAll',
  async (filter, { rejectWithValue }) => {
    try {
      const params = { year: filter?.Year ?? undefined, revTypeId: filter?.RevTypeId ?? undefined };
      return (await http.get<CL_Doc_Revisions[]>(RESOURCE, { params })).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

export const getRevisionById = createAsyncThunk<CL_Doc_Revisions, number, { rejectValue: ApiError }>(
  'revisions/getById',
  async (id, { rejectWithValue }) => {
    try {
      return (await http.get<CL_Doc_Revisions>(`${RESOURCE}/${id}`)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** POST api/revisions - needs UNOS, ADMIN or REVIZIJA. */
export const createRevision = createAsyncThunk<CL_Doc_Revisions, CL_Doc_Revisions, { rejectValue: ApiError }>(
  'revisions/create',
  async (revision, { rejectWithValue }) => {
    try {
      return (await http.post<CL_Doc_Revisions>(RESOURCE, revision)).data;
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

/** PUT api/revisions/{id} - HTTP 409 when row_version is stale (ApiError.currentRecord has the current row). */
export const updateRevision = createAsyncThunk<void, CL_Doc_Revisions, { rejectValue: ApiError }>(
  'revisions/update',
  async (revision, { rejectWithValue }) => {
    try {
      await http.put(`${RESOURCE}/${revision.CL_Doc_Revision_Id}`, revision);
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);

export const deleteRevision = createAsyncThunk<void, number, { rejectValue: ApiError }>(
  'revisions/delete',
  async (id, { rejectWithValue }) => {
    try {
      await http.delete(`${RESOURCE}/${id}`);
    } catch (error) {
      return rejectWithValue(toApiError(error));
    }
  },
);
