import { Box, Typography } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useMemo } from 'react';
import { useProcessingStatuses } from '../../../hooks/useReferenceData';
import { ReportRiskRow } from '../../../models';
import { formatDate } from '../../../utils/formatting';
import AppDataGrid from '../../shared/AppDataGrid';
import { RiskChip, StatusChip } from '../../shared/StatusChips';

type Props = {
  rows: ReportRiskRow[];
};

/** Grid shared by every risk-report panel on "Izvješća". The toolbar's export button gives a CSV of the visible rows. */
export default function ReportRiskTable({ rows }: Props) {
  const statuses = useProcessingStatuses();
  const columns = useMemo<GridColDef<ReportRiskRow>[]>(
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
            <Typography variant="caption" color="text.secondary" noWrap component="div">
              {params.row.VrstaKlijenta}
              {params.row.Oib ? ` · OIB ${params.row.Oib}` : ''}
            </Typography>
          </Box>
        ),
      },
      {
        field: 'ClntPrcsngSt',
        headerName: 'Status obrade',
        width: 160,
        renderCell: (params) => (
          <StatusChip code={params.row.ClntPrcsngSt} label={statuses?.find((s) => s.ClntPrcsStCd === params.row.ClntPrcsngSt)?.Status} />
        ),
      },
      { field: 'RiskLevel', headerName: 'Procjena', width: 170 },
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
      { field: 'AddDt', headerName: 'Datum unosa', width: 130, valueFormatter: (value: string) => formatDate(value) },
      { field: 'MdfDt', headerName: 'Zadnja izmjena', width: 140, valueFormatter: (value: string) => formatDate(value) },
    ],
    [statuses],
  );

  return <AppDataGrid rows={rows} columns={columns} getRowId={(row) => row.ClntId} getRowHeight={() => 56} />;
}
