import { Box, Typography } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useMemo, useState } from 'react';
import { DashboardClientRow } from '../../../models';
import { formatDateTime } from '../../../utils/formatting';
import AppDataGrid, { clickableRowsSx } from '../../shared/AppDataGrid';
import { RiskChip, StatusChip } from '../../shared/StatusChips';
import ClientDetailDrawer from '../dashboard/ClientDetailDrawer';

type Props = {
  rows: DashboardClientRow[];
};

/** Grid shared by every client-type tab of "Klijenti - analiza" and "Klijenti - pregled"; a row click opens the details panel. */
export default function ClientAnalysisTable({ rows }: Props) {
  const [selected, setSelected] = useState<DashboardClientRow | null>(null);

  const columns = useMemo<GridColDef<DashboardClientRow>[]>(
    () => [
      {
        field: 'ClntNm',
        headerName: 'Klijent',
        flex: 1.6,
        minWidth: 240,
        renderCell: (params) => (
          <Box sx={{ minWidth: 0 }}>
            <Typography variant="body2" noWrap sx={{ fontWeight: 600, lineHeight: 1.3 }}>
              {params.row.ClntNm}
            </Typography>
            {params.row.Oib && (
              <Typography variant="caption" color="text.secondary" noWrap component="div">
                OIB {params.row.Oib}
              </Typography>
            )}
          </Box>
        ),
      },
      {
        field: 'Status',
        headerName: 'Status obrade',
        width: 180,
        valueGetter: (_value, row) => row.Status ?? row.ClntPrcsngSt,
        renderCell: (params) => <StatusChip code={params.row.ClntPrcsngSt} label={params.row.Status} />,
      },
      {
        field: 'RskPnts',
        headerName: 'Rizik',
        width: 130,
        type: 'number',
        headerAlign: 'left',
        align: 'left',
        renderCell: (params) => <RiskChip points={params.row.RskPnts} />,
      },
      { field: 'PepInd', headerName: 'PEP', width: 80 },
      { field: 'WtchLstInd', headerName: 'Watchlist', width: 100 },
      {
        field: 'MdfDt',
        headerName: 'Zadnja izmjena',
        width: 200,
        valueGetter: (_value, row) => row.MdfDt,
        renderCell: (params) => (
          <Box sx={{ minWidth: 0 }}>
            <Typography variant="body2" noWrap sx={{ lineHeight: 1.3 }}>
              {formatDateTime(params.row.MdfDt)}
            </Typography>
            {params.row.ModifiedByName && (
              <Typography variant="caption" color="text.secondary" noWrap component="div">
                {params.row.ModifiedByName}
              </Typography>
            )}
          </Box>
        ),
      },
    ],
    [],
  );

  return (
    <>
      <AppDataGrid
        rows={rows}
        columns={columns}
        getRowId={(row) => row.ClntId}
        getRowHeight={() => 56}
        onRowClick={(params) => setSelected(params.row)}
        getRowClassName={(params) => (params.row.ClntId === selected?.ClntId ? 'selected-row' : '')}
        sx={clickableRowsSx}
      />
      <ClientDetailDrawer row={selected} onClose={() => setSelected(null)} />
    </>
  );
}
