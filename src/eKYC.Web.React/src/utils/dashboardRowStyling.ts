import { alpha, Theme } from '@mui/material/styles';
import { DashboardClientRow } from '../models';
import { HIGH_RISK_POINTS } from './statusColors';

/**
 * Row emphasis from the legacy dashboard (EkycUI.getDGRRowStyle() + mytheme.scss), same decision rules as the Blazor
 * DashboardRowStyling, but expressed as a soft tint + colored left edge instead of saturated full-row colors:
 *   rejected -> amber, needs modification / high-risk change -> red, active -> green, everything else -> neutral.
 * Colors come from the theme, so light and dark mode both stay readable.
 */
export type DashboardRowClass = 'row-rejected' | 'row-must-modify' | 'row-high-risk' | 'row-active' | 'row-default';

export function getDashboardRowClass(row: DashboardClientRow): DashboardRowClass {
  if (row.ClntPrcsngSt === 'ODBIJEN') return 'row-rejected';

  // Checked before the status-based branches, same as legacy - a stale zero score can override an AKTIVAN client.
  if (row.MustModify) return 'row-must-modify';

  if (row.ClntPrcsngSt === 'IZMJENA') {
    return (row.RskPnts ?? 0) < HIGH_RISK_POINTS ? 'row-default' : 'row-high-risk';
  }

  if (row.ClntPrcsngSt === 'AKTIVAN') return 'row-active';

  return 'row-default';
}

/** Master data (HBOR feed) differs from the local record and nobody told the system to ignore it. */
export const isMasterDataChanged = (row: DashboardClientRow): boolean => row.MpChange && !row.DoNotCheck;

/** Rows that need attention first: red-flagged or high-risk changes. */
export const needsAttention = (row: DashboardClientRow): boolean => {
  const cls = getDashboardRowClass(row);
  return cls === 'row-must-modify' || cls === 'row-high-risk';
};

const tinted = (theme: Theme, color: string, strength: number) => ({
  backgroundColor: alpha(color, theme.palette.mode === 'light' ? strength : strength + 0.06),
  '& .MuiDataGrid-cell[data-colindex="0"]': { boxShadow: `inset 4px 0 0 ${color}` },
  '&:hover': { backgroundColor: alpha(color, theme.palette.mode === 'light' ? strength + 0.06 : strength + 0.12) },
});

/** Pass as (part of) the `sx` of a DataGrid that uses getDashboardRowClass for getRowClassName. */
export const dashboardGridSx = (theme: Theme) => ({
  '& .row-rejected': tinted(theme, theme.palette.warning.main, 0.1),
  '& .row-must-modify': tinted(theme, theme.palette.error.main, 0.1),
  '& .row-high-risk': tinted(theme, theme.palette.error.main, 0.1),
  '& .row-active': tinted(theme, theme.palette.success.main, 0.08),
  '& .MuiDataGrid-row': { cursor: 'pointer' },
  '& .selected-row, & .selected-row:hover': { outline: `2px solid ${theme.palette.primary.main}`, outlineOffset: -2 },
});

export const ROW_LEGEND: { label: string; color: 'error' | 'warning' | 'success' }[] = [
  { label: 'Treba izmjenu / izmjena uz visok rizik', color: 'error' },
  { label: 'Odbijen', color: 'warning' },
  { label: 'Aktivan', color: 'success' },
];
