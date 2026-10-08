import { Chip } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useMemo } from 'react';
import { DashboardClientRow } from '../../../models';
import { formatDateTime } from '../../../utils/formatting';
import AppDataGrid from '../../shared/AppDataGrid';

type Props = {
  rows: DashboardClientRow[];
};

/** Grid shared by every client-type tab of "Klijenti - analiza" and "Klijenti - pregled". */
export default function ClientAnalysisTable({ rows }: Props) {
  const columns = useMemo<GridColDef<DashboardClientRow>[]>(
    () => [
      { field: 'ClntNm', headerName: 'Naziv klijenta', flex: 1.5, minWidth: 200 },
      { field: 'Oib', headerName: 'OIB', width: 130 },
      {
        field: 'Status',
        headerName: 'Status obrade',
        width: 170,
        valueGetter: (_value, row) => row.Status ?? row.ClntPrcsngSt,
        renderCell: (params) => <Chip size="small" color="info" label={String(params.value ?? '')} />,
      },
      { field: 'RskPnts', headerName: 'Rizik (bodovi)', type: 'number', width: 130 },
      { field: 'PepInd', headerName: 'PEP', width: 90 },
      { field: 'WtchLstInd', headerName: 'Watchlist', width: 100 },
      {
        field: 'MdfDt',
        headerName: 'Zadnja izmjena',
        width: 270,
        valueGetter: (_value, row) => row.MdfDt,
        renderCell: (params) =>
          `${formatDateTime(params.row.MdfDt)}${params.row.ModifiedByName ? ` (${params.row.ModifiedByName})` : ''}`,
      },
    ],
    [],
  );

  return <AppDataGrid rows={rows} columns={columns} getRowId={(row) => row.ClntId} />;
}
