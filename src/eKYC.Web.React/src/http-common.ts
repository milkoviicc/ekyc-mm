import axios, { AxiosError } from 'axios';
import { ConcurrencyConflict } from './models';

/** Empty = same origin; in dev the Vite proxy forwards /api to eKYC.Api (see vite.config.ts). */
export const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? '';

const http = axios.create({
  baseURL: BASE_URL,
  // Needed once the API sits behind Windows/Negotiate auth on IIS (credentials flow with the request).
  withCredentials: true,
  // Arrays are sent as repeated keys (clntTypCds=P&clntTypCds=L), which is what ASP.NET Core model binding expects.
  paramsSerializer: { indexes: null },
});

export default http;

/** Normalized failure shape that every thunk rejects with. */
export type ApiError = {
  status?: number;
  message: string;
  /** Present on HTTP 409: the freshly reloaded record from the server. */
  currentRecord?: unknown;
};

export const CONFLICT = 409;

export function toApiError(error: unknown): ApiError {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<Partial<ConcurrencyConflict<unknown>> | string>;
    const data = axiosError.response?.data;
    const message =
      typeof data === 'string' && data.length > 0
        ? data
        : typeof data === 'object' && data !== null && typeof data.message === 'string'
          ? data.message
          : axiosError.message;
    return {
      status: axiosError.response?.status,
      message,
      currentRecord: typeof data === 'object' && data !== null ? data.currentRecord : undefined,
    };
  }
  return { message: error instanceof Error ? error.message : 'Unknown error' };
}

export const isConflict = (error: unknown): boolean => (error as ApiError | undefined)?.status === CONFLICT;
