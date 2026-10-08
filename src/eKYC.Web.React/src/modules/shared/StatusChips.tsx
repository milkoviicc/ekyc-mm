import { Chip, ChipProps } from '@mui/material';
import { processingStatusColor, RISK_BAND_COLOR, RISK_BAND_LABEL, riskBand } from '../../utils/statusColors';

type StatusChipProps = {
  /** CL_Clnt_PrcsSt code (drives the color). */
  code: string;
  /** Display name; falls back to the code. */
  label?: string | null;
} & Omit<ChipProps, 'label' | 'color'>;

/** Processing-status pill, colored by meaning (active = green, change requested = amber, rejected = red, ...). */
export function StatusChip({ code, label, ...props }: StatusChipProps) {
  const color = processingStatusColor(code);
  return <Chip size="small" variant={color === 'default' ? 'outlined' : 'filled'} color={color} label={label ?? code} {...props} />;
}

type RiskChipProps = {
  points?: number | null;
  /** Show the number next to the band name. */
  showPoints?: boolean;
} & Omit<ChipProps, 'label' | 'color'>;

/** Risk pill: band (nizak/srednji/visok) + points, colored by band. */
export function RiskChip({ points, showPoints = true, ...props }: RiskChipProps) {
  const band = riskBand(points);
  if (band === 'none') return <span aria-label="Nema bodova">—</span>;
  // 0 points = not scored yet (typical for clients still in the work queue), so no risk color is implied.
  if (points === 0) return <Chip size="small" variant="outlined" label="0" aria-label="Nije ocijenjeno" {...props} />;
  return (
    <Chip
      size="small"
      variant="outlined"
      color={RISK_BAND_COLOR[band]}
      label={showPoints ? `${RISK_BAND_LABEL[band]} · ${points}` : RISK_BAND_LABEL[band]}
      {...props}
    />
  );
}

/** Small yes/no marker for PEP and watchlist columns - shows nothing noisy for the common "no" case. */
export function FlagChip({ active, label }: { active: boolean; label: string }) {
  if (!active) return <span style={{ opacity: 0.45 }}>—</span>;
  return <Chip size="small" color="error" variant="outlined" label={label} />;
}
