import AutorenewIcon from '@mui/icons-material/Autorenew';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutlineOutlined';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutlineOutlined';
import HourglassEmptyIcon from '@mui/icons-material/HourglassEmpty';
import RefreshIcon from '@mui/icons-material/Refresh';
import SyncProblemIcon from '@mui/icons-material/SyncProblem';
import { Alert, Box, Button, Chip, Stack, TextField, Tooltip, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { GridColDef } from '@mui/x-data-grid';
import { useEffect, useMemo, useState } from 'react';
import { useClientTypes, useProcessingStatuses, useRiskEstimates } from '../../../hooks/useReferenceData';
import { DashboardClientRow, DashboardFilter } from '../../../models';
import { getWorkQueue } from '../../../store/dashboard';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import {
  dashboardGridSx,
  getDashboardRowClass,
  isMasterDataChanged,
  needsAttention,
  ROW_LEGEND,
} from '../../../utils/dashboardRowStyling';
import { formatDateTime } from '../../../utils/formatting';
import { HIGH_RISK_POINTS } from '../../../utils/statusColors';
import AppDataGrid from '../../shared/AppDataGrid';
import EmptyState from '../../shared/EmptyState';
import FilterBar, { ActiveFilter } from '../../shared/FilterBar';
import KpiCard from '../../shared/KpiCard';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import { SelectField } from '../../shared/SelectField';
import { RiskChip, StatusChip } from '../../shared/StatusChips';
import ClientDetailDrawer from './ClientDetailDrawer';

type QuickFilter = 'all' | 'highRisk' | 'attention' | 'master';

const QUICK_PREDICATES: Record<QuickFilter, (row: DashboardClientRow) => boolean> = {
  all: () => true,
  highRisk: (r) => (r.RskPnts ?? 0) >= HIGH_RISK_POINTS,
  attention: needsAttention,
  master: isMasterDataChanged,
};

/** "Nadzorna ploča" - clients waiting to be processed (active/closed/stopped/rejected clients are not listed). */
export default function Dashboard() {
  const dispatch = useAppDispatch();
  const theme = useTheme();
  const { rows, pending, error } = useAppSelector((state) => state.dashboard);
  const clientTypes = useClientTypes();
  const processingStatuses = useProcessingStatuses();
  const riskEstimates = useRiskEstimates();

  const [draft, setDraft] = useState<DashboardFilter>({});
  const [applied, setApplied] = useState<DashboardFilter>({});
  const [quick, setQuick] = useState<QuickFilter>('all');
  const [selected, setSelected] = useState<DashboardClientRow | null>(null);

  useEffect(() => {
    dispatch(getWorkQueue({}));
  }, [dispatch]);

  const apply = (filter: DashboardFilter) => {
    setDraft(filter);
    setApplied(filter);
    setSelected(null);
    dispatch(getWorkQueue(filter));
  };

  const removeFilter = (key: keyof DashboardFilter) => apply({ ...applied, [key]: null });

  const activeFilters: ActiveFilter[] = [];
  const addActive = (key: keyof DashboardFilter, label: string | undefined) => {
    if (label) activeFilters.push({ key, label, onDelete: () => removeFilter(key) });
  };
  addActive('ClntTypCd', applied.ClntTypCd ? `Tip: ${clientTypes?.find((c) => c.ClntTypCd === applied.ClntTypCd)?.ClntTypDspn ?? applied.ClntTypCd}` : undefined);
  addActive('ClntNm', applied.ClntNm?.trim() ? `Naziv: ${applied.ClntNm}` : undefined);
  addActive(
    'ClntPrcsngSt',
    applied.ClntPrcsngSt
      ? `Status: ${processingStatuses?.find((p) => p.ClntPrcsStCd === applied.ClntPrcsngSt)?.Status ?? applied.ClntPrcsngSt}`
      : undefined,
  );
  addActive('Oib', applied.Oib?.trim() ? `OIB: ${applied.Oib}` : undefined);
  addActive(
    'RskEstId',
    applied.RskEstId !== null && applied.RskEstId !== undefined
      ? `Rizik: ${riskEstimates?.find((r) => r.RskEstId === applied.RskEstId)?.RiskLevel ?? applied.RskEstId}`
      : undefined,
  );

  const counts = useMemo(() => {
    const all = rows ?? [];
    return {
      all: all.length,
      highRisk: all.filter(QUICK_PREDICATES.highRisk).length,
      attention: all.filter(QUICK_PREDICATES.attention).length,
      master: all.filter(QUICK_PREDICATES.master).length,
    };
  }, [rows]);

  const visibleRows = useMemo(() => (rows ?? []).filter(QUICK_PREDICATES[quick]), [rows, quick]);
  const toggleQuick = (next: QuickFilter) => setQuick((current) => (current === next ? 'all' : next));

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
            <Typography variant="caption" color="text.secondary" noWrap component="div">
              {params.row.VrstaKlijenta}
              {params.row.Oib ? ` · OIB ${params.row.Oib}` : ''}
            </Typography>
          </Box>
        ),
      },
      {
        field: 'HborId',
        headerName: 'HBOR ID',
        width: 120,
        renderCell: (params) => (
          <Stack direction="row" spacing={0.75} sx={{ alignItems: "center" }}>
            <span>{params.row.HborId}</span>
            {isMasterDataChanged(params.row) && (
              <Tooltip title="Matični podaci se razlikuju od HBOR izvora">
                <SyncProblemIcon fontSize="small" color="warning" aria-label="Promjena matičnih podataka" />
              </Tooltip>
            )}
          </Stack>
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

  const reload = () => dispatch(getWorkQueue(applied));

  return (
    <>
      <PageHeader
        title="Nadzorna ploča"
        subtitle="Klijenti koji čekaju obradu. Aktivni, zatvoreni, prekinuti i odbijeni klijenti se ovdje ne prikazuju."
        actions={
          <Button variant="outlined" color="inherit" startIcon={<RefreshIcon />} onClick={reload} sx={{ borderColor: 'divider' }}>
            Osvježi
          </Button>
        }
      />

      <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(230px, 1fr))', gap: 2, mb: 2 }}>
        <KpiCard label="Čeka obradu" value={counts.all} icon={<HourglassEmptyIcon />} selected={quick === 'all'} onClick={() => setQuick('all')} />
        <KpiCard
          label="Visok rizik"
          hint={`${HIGH_RISK_POINTS}+ bodova`}
          value={counts.highRisk}
          color="error"
          icon={<ErrorOutlineIcon />}
          selected={quick === 'highRisk'}
          onClick={() => toggleQuick('highRisk')}
        />
        <KpiCard
          label="Treba izmjenu"
          hint="uključuje izmjene uz visok rizik"
          value={counts.attention}
          color="warning"
          icon={<AutorenewIcon />}
          selected={quick === 'attention'}
          onClick={() => toggleQuick('attention')}
        />
        <KpiCard
          label="Promjena matičnih podataka"
          hint="razlika prema HBOR izvoru"
          value={counts.master}
          color="info"
          icon={<SyncProblemIcon />}
          selected={quick === 'master'}
          onClick={() => toggleQuick('master')}
        />
      </Box>

      <FilterBar active={activeFilters} onApply={() => apply(draft)} onClear={() => apply({})}>
        <SelectField
          label="Tip klijenta"
          value={draft.ClntTypCd ?? null}
          onChange={(v) => setDraft({ ...draft, ClntTypCd: v })}
          options={(clientTypes ?? []).map((ct) => ({ value: ct.ClntTypCd, label: ct.ClntTypDspn ?? ct.ClntTypCd }))}
        />
        <TextField label="Naziv klijenta" value={draft.ClntNm ?? ''} onChange={(e) => setDraft({ ...draft, ClntNm: e.target.value })} />
        <SelectField
          label="Status obrade"
          value={draft.ClntPrcsngSt ?? null}
          onChange={(v) => setDraft({ ...draft, ClntPrcsngSt: v })}
          options={(processingStatuses ?? []).map((ps) => ({ value: ps.ClntPrcsStCd, label: ps.Status ?? ps.ClntPrcsStCd }))}
        />
        <TextField label="OIB" value={draft.Oib ?? ''} onChange={(e) => setDraft({ ...draft, Oib: e.target.value })} />
        <SelectField
          label="Rizik"
          value={draft.RskEstId ?? null}
          onChange={(v) => setDraft({ ...draft, RskEstId: v })}
          options={(riskEstimates ?? []).map((re) => ({ value: re.RskEstId, label: re.RiskLevel ?? String(re.RskEstId) }))}
        />
      </FilterBar>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} action={<Button color="inherit" size="small" onClick={reload}>Pokušaj ponovno</Button>}>
          {error}
        </Alert>
      )}

      {pending || rows === null ? (
        error ? null : <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState
          icon={<CheckCircleOutlineIcon />}
          title="Nema klijenata koji čekaju obradu"
          description={activeFilters.length ? 'Nijedan klijent ne odgovara odabranim filtrima.' : 'Sve je obrađeno.'}
          action={activeFilters.length ? <Button onClick={() => apply({})}>Očisti filtre</Button> : undefined}
        />
      ) : (
        <>
          <AppDataGrid
            rows={visibleRows}
            columns={columns}
            getRowId={(row) => row.ClntId}
            getRowHeight={() => 56}
            onRowClick={(params) => setSelected(params.row)}
            getRowClassName={(params) =>
              `${getDashboardRowClass(params.row)}${params.row.ClntId === selected?.ClntId ? ' selected-row' : ''}`
            }
            sx={dashboardGridSx(theme)}
            emptyTitle="Nema klijenata u ovom prikazu"
          />
          <Stack direction="row" spacing={2} useFlexGap sx={{ mt: 1.5, flexWrap: "wrap", alignItems: "center" }}>
            <Typography variant="caption" color="text.secondary">
              Boja ruba retka:
            </Typography>
            {ROW_LEGEND.map((l) => (
              <Chip key={l.label} size="small" variant="outlined" color={l.color} label={l.label} />
            ))}
          </Stack>
        </>
      )}

      <ClientDetailDrawer row={selected} onClose={() => setSelected(null)} />
    </>
  );
}
