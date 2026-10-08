import { Alert, Box, Button, Paper, Tab, Tabs, TextField } from '@mui/material';
import { useEffect, useState } from 'react';
import { useProcessingStatuses, useRiskEstimates } from '../../../hooks/useReferenceData';
import { DashboardFilter } from '../../../models';
import { getClientAnalysis, resetClientAnalysis } from '../../../store/clientAnalysis';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import FilterGrid from '../../shared/FilterGrid';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import { SelectField } from '../../shared/SelectField';
import ClientAnalysisTable from './ClientAnalysisTable';

/** Tab index -> CL_Clnt_Typ code. Order matches the tab labels below. */
const TABS = [
  { clntTypCd: 'P', label: 'Fizičke osobe' },
  { clntTypCd: 'L', label: 'Pravne osobe' },
  { clntTypCd: 'BEU', label: 'Banke EU' },
  { clntTypCd: 'BINT', label: 'Banke izvan EU' },
] as const;

type Props = {
  title: string;
  subtitle: string;
};

/**
 * Shared implementation of "Klijenti - analiza" and "Klijenti - pregled": both bind to the same endpoint
 * (api/client-analysis); the legacy app only differs by disabling row-click-to-edit, which is not built yet.
 */
export default function ClientAnalysisBase({ title, subtitle }: Props) {
  const dispatch = useAppDispatch();
  const { rowsByType, pendingTypes, error } = useAppSelector((state) => state.clientAnalysis);
  const processingStatuses = useProcessingStatuses();
  const riskEstimates = useRiskEstimates();

  const [filter, setFilter] = useState<DashboardFilter>({});
  const [activeIndex, setActiveIndex] = useState(0);

  // Tabs load lazily: only the visible one is fetched, and a result stays cached until the filter changes.
  useEffect(() => {
    const { clntTypCd } = TABS[activeIndex];
    if (rowsByType[clntTypCd] === undefined && !pendingTypes.includes(clntTypCd)) {
      dispatch(getClientAnalysis({ clntTypCd, filter }));
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps -- `filter` is applied only via the buttons below
  }, [activeIndex, rowsByType, pendingTypes, dispatch]);

  useEffect(() => {
    dispatch(resetClientAnalysis());
  }, [dispatch]);

  const apply = (f: DashboardFilter) => {
    setFilter(f);
    dispatch(resetClientAnalysis());
    // The effect above re-fetches the active tab because its cache entry was cleared; pass the new filter explicitly.
    dispatch(getClientAnalysis({ clntTypCd: TABS[activeIndex].clntTypCd, filter: f }));
  };

  const active = TABS[activeIndex];
  const rows = rowsByType[active.clntTypCd];

  return (
    <>
      <PageHeader title={title} subtitle={subtitle} />

      <Paper sx={{ p: 2, mb: 2 }} elevation={1}>
        <FilterGrid>
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
          <Button variant="contained" onClick={() => apply(filter)}>
            Primjeni filtar
          </Button>
          <Button variant="outlined" onClick={() => apply({})}>
            Isprazni filtar
          </Button>
        </Box>
      </Paper>

      <Paper elevation={1}>
        <Tabs value={activeIndex} onChange={(_, index: number) => setActiveIndex(index)} variant="scrollable">
          {TABS.map((tab) => (
            <Tab key={tab.clntTypCd} label={tab.label} />
          ))}
        </Tabs>
        <Box sx={{ p: 2 }}>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {error}
            </Alert>
          )}
          {rows === undefined ? (
            error ? null : (
              <LoadingBlock />
            )
          ) : rows.length === 0 ? (
            <Alert severity="info">Nema klijenata za odabrane kriterije.</Alert>
          ) : (
            <ClientAnalysisTable rows={rows} />
          )}
        </Box>
      </Paper>
    </>
  );
}
