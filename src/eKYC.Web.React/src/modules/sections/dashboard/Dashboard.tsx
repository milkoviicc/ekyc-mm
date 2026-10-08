import RefreshIcon from '@mui/icons-material/Refresh';
import { Alert, Box, Button, Chip, Paper, TextField, Typography } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useEffect, useMemo, useState } from 'react';
import { useClientTypes, useProcessingStatuses, useRiskEstimates } from '../../../hooks/useReferenceData';
import { DashboardClientRow, DashboardFilter } from '../../../models';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { getWorkQueue } from '../../../store/dashboard';
import { formatDateTime } from '../../../utils/formatting';
import {
  dashboardGridSx,
  getDashboardRowClass,
  isHborIdHighlighted,
} from '../../../utils/dashboardRowStyling';
import AppDataGrid from '../../shared/AppDataGrid';
import FilterGrid from '../../shared/FilterGrid';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import { SelectField } from '../../shared/SelectField';

/** "Nadzorna ploča" - clients waiting to be processed (active/closed/stopped/rejected clients are not listed). */
export default function Dashboard() {
  const dispatch = useAppDispatch();
  const { rows, pending, error } = useAppSelector((state) => state.dashboard);
  const clientTypes = useClientTypes();
  const processingStatuses = useProcessingStatuses();
  const riskEstimates = useRiskEstimates();

  const [filter, setFilter] = useState<DashboardFilter>({});

  useEffect(() => {
    dispatch(getWorkQueue({}));
  }, [dispatch]);

  const load = (f: DashboardFilter) => dispatch(getWorkQueue(f));

  const clearFilter = () => {
    setFilter({});
    load({});
  };

  const columns = useMemo<GridColDef<DashboardClientRow>[]>(
    () => [
      {
        field: 'HborId',
        headerName: 'HBOR ID',
        width: 110,
        cellClassName: (params) => (isHborIdHighlighted(params.row) ? 'cell-hbor-changed' : ''),
      },
      { field: 'ClntNm', headerName: 'Naziv klijenta', flex: 1.5, minWidth: 200 },
      { field: 'VrstaKlijenta', headerName: 'Vrsta', width: 140 },
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
        valueFormatter: (value: string) => formatDateTime(value),
        renderCell: (params) =>
          `${formatDateTime(params.row.MdfDt)}${params.row.ModifiedByName ? ` (${params.row.ModifiedByName})` : ''}`,
      },
    ],
    [],
  );

  return (
    <>
      <PageHeader
        title="Nadzorna ploča"
        subtitle="Klijenti koji čekaju obradu — aktivni, zatvoreni, prekinuti i odbijeni klijenti se ovdje ne prikazuju."
      />

      <Paper sx={{ p: 2, mb: 2 }} elevation={1}>
        <FilterGrid>
          <SelectField
            label="Tip klijenta"
            value={filter.ClntTypCd ?? null}
            onChange={(v) => setFilter({ ...filter, ClntTypCd: v })}
            options={(clientTypes ?? []).map((ct) => ({ value: ct.ClntTypCd, label: ct.ClntTypDspn ?? ct.ClntTypCd }))}
          />
          <TextField
            size="small"
            label="Naziv klijenta"
            value={filter.ClntNm ?? ''}
            onChange={(e) => setFilter({ ...filter, ClntNm: e.target.value })}
          />
          <SelectField
            label="Status obrade"
            value={filter.ClntPrcsngSt ?? null}
            onChange={(v) => setFilter({ ...filter, ClntPrcsngSt: v })}
            options={(processingStatuses ?? []).map((ps) => ({
              value: ps.ClntPrcsStCd,
              label: ps.Status ?? ps.ClntPrcsStCd,
            }))}
          />
          <TextField
            size="small"
            label="OIB"
            value={filter.Oib ?? ''}
            onChange={(e) => setFilter({ ...filter, Oib: e.target.value })}
          />
          <SelectField
            label="Rizik"
            value={filter.RskEstId ?? null}
            onChange={(v) => setFilter({ ...filter, RskEstId: v })}
            options={(riskEstimates ?? []).map((re) => ({ value: re.RskEstId, label: re.RiskLevel ?? String(re.RskEstId) }))}
          />
        </FilterGrid>
        <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
          <Button variant="contained" onClick={() => load(filter)}>
            Primjeni filtar
          </Button>
          <Button variant="outlined" onClick={clearFilter}>
            Isprazni filtar
          </Button>
          <Box sx={{ flex: 1 }} />
          <Button variant="outlined" startIcon={<RefreshIcon />} onClick={() => load(filter)}>
            Osvježi prikaz
          </Button>
        </Box>
      </Paper>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      {pending || rows === null ? (
        error ? null : (
          <LoadingBlock />
        )
      ) : rows.length === 0 ? (
        <Alert severity="success">Nema klijenata koji čekaju obradu.</Alert>
      ) : (
        <>
          <AppDataGrid
            rows={rows}
            columns={columns}
            getRowId={(row) => row.ClntId}
            getRowClassName={(params) => getDashboardRowClass(params.row)}
            sx={dashboardGridSx}
          />
          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 1 }}>
            Boja retka odražava status obrade i rizik (zlatna = odbijen, tamnocrvena = izmjena uz visok rizik, blijedozelena = aktivan,
            blijedožuta = ostalo). Ljubičasta HBOR ID ćelija označava klijente kod kojih se matični podaci razlikuju od HBOR izvora.
          </Typography>
        </>
      )}
    </>
  );
}
