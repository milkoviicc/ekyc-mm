/** Body of an HTTP 409 from eKYC.Api (ExceptionHandlingMiddleware) when a row_version check fails. */
export type ConcurrencyConflict<T> = {
  error: 'concurrency_conflict';
  message: string;
  /** The freshly reloaded current record. */
  currentRecord: T | null;
};
