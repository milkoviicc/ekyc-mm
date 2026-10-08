import type { ChipProps } from '@mui/material';

type ChipColor = NonNullable<ChipProps['color']>;

/**
 * Semantic color for a client processing status (CL_Clnt_PrcsSt code). Unknown codes fall back to "default" so a new
 * status added in the database still renders, just without a color.
 */
const PROCESSING_STATUS_COLORS: Record<string, ChipColor> = {
  NOVI: 'info',
  'UPITNIK KREIRAN': 'info',
  CEKANJE: 'warning',
  IZMJENA: 'warning',
  ODOBRENJE1: 'primary',
  ODOBRENJE2: 'primary',
  AKTIVAN: 'success',
  ODBIJEN: 'error',
  ZATVOREN: 'default',
  PREKID: 'default',
};

export const processingStatusColor = (code: string): ChipColor => PROCESSING_STATUS_COLORS[code] ?? 'default';

/** Risk bands follow CL_Rsk_Est: 0-3 low, 4-6 medium, 7+ high. */
export type RiskBand = 'none' | 'low' | 'medium' | 'high';

export const HIGH_RISK_POINTS = 7;

export function riskBand(points?: number | null): RiskBand {
  if (points === null || points === undefined) return 'none';
  if (points >= HIGH_RISK_POINTS) return 'high';
  if (points >= 4) return 'medium';
  return 'low';
}

export const RISK_BAND_COLOR: Record<RiskBand, ChipColor> = {
  none: 'default',
  low: 'success',
  medium: 'warning',
  high: 'error',
};

export const RISK_BAND_LABEL: Record<RiskBand, string> = {
  none: '—',
  low: 'Nizak',
  medium: 'Srednji',
  high: 'Visok',
};
