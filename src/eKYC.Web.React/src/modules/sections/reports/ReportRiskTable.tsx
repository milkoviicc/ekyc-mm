import { Chip } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useMemo } from 'react';
import { ReportRiskRow } from '../../../models';
import { formatDate } from '../../../utils/formatting';
import AppDataGrid from '../../shared/AppDataGrid';

type Props = {
  rows: ReportRiskRow[];
};

/** Grid shared by every risk-report panel on "Izvješća". */
export default function ReportRiskTable({ rows }: Props) {
  const columns = useMemo<GridColDef<ReportRiskRow>[]>(
    () => [
      { field: 'ClntNm', headerName: 'Naziv klijenta', flex: 1.5, minWidth: 200 },
      { field: 'VrstaKlijenta', headerName: 'Vrsta', width: 140 },
      { field: 'Oib', headerName: 'OIB', width: 130 },
      {
        field: 'ClntPrcsngSt',
        headerName: 'Status obrade',
        width: 160,
        renderCell: (params) => <Chip size="small" color="info" label={String(params.value ?? '')} />,
      },
      { field: 'RiskLevel', headerName: 'Rizik', width: 130 },
      { field: 'RskPnts', headerName: 'Bodovi', type: 'number', width: 90 },
      { field: 'PepInd', headerName: 'PEP', width: 90 },
      { field: 'WtchLstInd', headerName: 'Watchlist', width: 100 },
      {
        field: 'AddDt',
        headerName: 'Datum unosa',
        width: 130,
        valueFormatter: (value: string) => formatDate(value),
      },
      {
        field: 'MdfDt',
        headerName: 'Zadnja izmjena',
        width: 140,
        valueFormatter: (value: string) => formatDate(value),
      },
    ],
    [],
  );

  return <AppDataGrid rows={rows} columns={columns} getRowId={(row) => row.ClntId} />;
}
