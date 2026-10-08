import { DashboardClientRow } from '../models';

/**
 * Row/cell colors from the legacy dashboard (EkycUI.getDGRRowStyle() + mytheme.scss), same rules as the Blazor
 * DashboardRowStyling. Returned as a CSS class name; the colors themselves live in dashboardGridSx below.
 */
export type DashboardRowClass = 'row-rejected' | 'row-must-modify' | 'row-high-risk' | 'row-active' | 'row-default';

export function getDashboardRowClass(row: DashboardClientRow): DashboardRowClass {
  if (row.ClntPrcsngSt === 'ODBIJEN') return 'row-rejected';

  // Checked before the status-based branches, same as legacy - a stale zero score can override an AKTIVAN client.
  if (row.MustModify) return 'row-must-modify';

  if (row.ClntPrcsngSt === 'IZMJENA') {
    return (row.RskPnts ?? 0) < 7 ? 'row-default' : 'row-high-risk';
  }

  if (row.ClntPrcsngSt === 'AKTIVAN') return 'row-active';

  return 'row-default';
}

/** HBOR_ID cell: purple when a pending master-data change applies, otherwise it just matches the row. */
export const isHborIdHighlighted = (row: DashboardClientRow): boolean => row.MpChange && !row.DoNotCheck;

const rowColors = (background: string, color: string) => ({
  backgroundColor: background,
  color,
  // Every cell gets the colors itself (not just the row) - cell-level styles would otherwise win.
  '& .MuiDataGrid-cell': { backgroundColor: background, color },
  '&:hover': { backgroundColor: background, filter: 'brightness(0.95)' },
});

/** Pass as the `sx` of a DataGrid that uses getDashboardRowClass for getRowClassName. */
export const dashboardGridSx = {
  '& .row-rejected': rowColors('gold', 'black'),
  '& .row-must-modify': rowColors('darkred', 'white'),
  '& .row-high-risk': rowColors('darkred', 'white'),
  '& .row-active': rowColors('palegreen', 'black'),
  '& .row-default': rowColors('lemonchiffon', 'black'),
  '& .MuiDataGrid-row .MuiDataGrid-cell.cell-hbor-changed': { backgroundColor: '#D7C4FF', color: 'black' },
};
