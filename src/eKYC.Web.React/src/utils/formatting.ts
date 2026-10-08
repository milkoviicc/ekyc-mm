import dayjs from 'dayjs';

export const DATE_FORMAT = 'DD.MM.YYYY.';
export const DATE_TIME_FORMAT = 'DD.MM.YYYY. HH:mm';

export const formatDate = (iso?: string | null): string => (iso ? dayjs(iso).format(DATE_FORMAT) : '');

export const formatDateTime = (iso?: string | null): string => (iso ? dayjs(iso).format(DATE_TIME_FORMAT) : '');

/** Full name from the joined A_Usr columns, e.g. "Ana Anić". */
export const fullName = (first?: string | null, last?: string | null): string =>
  [first, last].filter((part) => part && part.trim().length > 0).join(' ');

/** Sends a calendar day to the API as a date-time at local midnight, with no time-zone shift. */
export const toApiDate = (value: dayjs.Dayjs | null | undefined): string | null =>
  value ? value.format('YYYY-MM-DDT00:00:00') : null;
