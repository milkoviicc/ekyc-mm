import PersonSearchOutlinedIcon from '@mui/icons-material/PersonSearchOutlined';
import { Alert, Box, Button, Paper, Tab, Tabs, TextField } from '@mui/material';
import { useEffect, useState } from 'react';
import { useProcessingStatuses, useRiskEstimates } from '../../../hooks/useReferenceData';
import { useUrlTab } from '../../../hooks/useUrlTab';
import { DashboardFilter } from '../../../models';
import { getClientAnalysis, resetClientAnalysis } from '../../../store/clientAnalysis';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import EmptyState from '../../shared/EmptyState';
import FilterBar, { ActiveFilter } from '../../shared/FilterBar';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import { SelectField } from '../../shared/SelectField';
import ClientAnalysisTable from './ClientAnalysisTable';

/** Tab id (used in the URL) -> CL_Clnt_Typ code. Order matches the tab labels. */
const TABS = [
  { id: 'fizicke', clntTypCd: 'P', label: 'Fizičke osobe' },
  { id: 'pravne', clntTypCd: 'L', label: 'Pravne osobe' },
  { id: 'banke-eu', clntTypCd: 'BEU', label: 'Banke EU' },
  { id: 'banke-izvan-eu', clntTypCd: 'BINT', label: 'Banke izvan EU' },
] as const;
const TAB_IDS = TABS.map((t) => t.id);

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

  const [draft, setDraft] = useState<DashboardFilter>({});
  const [applied, setApplied] = useState<DashboardFilter>({});
  const [activeIndex, setActiveIndex] = useUrlTab(TAB_IDS);

  // A fresh visit starts from an empty cache, so the tabs always match the (empty) filter shown.
  useEffect(() => {
    dispatch(resetClientAnalysis());
  }, [dispatch]);

  // Tabs load lazily: only the visible one is fetched, and a result stays cached until the filter changes.
  useEffect(() => {
    const { clntTypCd } = TABS[activeIndex];
    // `error` stops a failing request from being retried in a tight loop; the alert below offers a manual retry.
    if (rowsByType[clntTypCd] === undefined && !pendingTypes.includes(clntTypCd) && !error) {
      dispatch(getClientAnalysis({ clntTypCd, filter: applied }));
    }
  }, [activeIndex, rowsByType, pendingTypes, applied, error, dispatch]);

  const apply = (filter: DashboardFilter) => {
    setDraft(filter);
    setApplied(filter);
    dispatch(resetClientAnalysis()); // the effect above re-fetches the active tab with the new filter
  };

  const active: ActiveFilter[] = [];
  if (applied.ClntNm?.trim()) active.push({ key: 'nm', label: `Naziv: ${applied.ClntNm}`, onDelete: () => apply({ ...applied, ClntNm: null }) });
  if (applied.ClntPrcsngSt) {
    const name = processingStatuses?.find((p) => p.ClntPrcsStCd === applied.ClntPrcsngSt)?.Status ?? applied.ClntPrcsngSt;
    active.push({ key: 'st', label: `Status: ${name}`, onDelete: () => apply({ ...applied, ClntPrcsngSt: null }) });
  }
  if (applied.Oib?.trim()) active.push({ key: 'oib', label: `OIB: ${applied.Oib}`, onDelete: () => apply({ ...applied, Oib: null }) });
  if (applied.RskEstId !== null && applied.RskEstId !== undefined) {
    const name = riskEstimates?.find((r) => r.RskEstId === applied.RskEstId)?.RiskLevel ?? applied.RskEstId;
    active.push({ key: 'rsk', label: `Rizik: ${name}`, onDelete: () => apply({ ...applied, RskEstId: null }) });
  }

  const rows = rowsByType[TABS[activeIndex].clntTypCd];

  return (
    <>
      <PageHeader title={title} subtitle={subtitle} />

      <FilterBar active={active} onApply={() => apply(draft)} onClear={() => apply({})}>
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

      <Paper variant="outlined" sx={{ overflow: 'hidden' }}>
        <Tabs value={activeIndex} onChange={(_, index: number) => setActiveIndex(index)} variant="scrollable" sx={{ px: 1, borderBottom: 1, borderColor: 'divider' }}>
          {TABS.map((tab) => (
            <Tab
              key={tab.id}
              label={tab.label}
              // Counts appear once a tab has been loaded - no extra requests just to fill the labels.
              iconPosition="end"
              icon={
                rowsByType[tab.clntTypCd] ? (
                  <Box component="span" sx={{ ml: 0.5, px: 0.75, borderRadius: 1, bgcolor: 'action.hover', fontSize: 12 }}>
                    {rowsByType[tab.clntTypCd]?.length}
                  </Box>
                ) : undefined
              }
            />
          ))}
        </Tabs>
        <Box sx={{ p: 2 }}>
          {error && (
            <Alert severity="error" sx={{ mb: 2 }} action={<Button color="inherit" size="small" onClick={() => apply(applied)}>Pokušaj ponovno</Button>}>
              {error}
            </Alert>
          )}
          {rows === undefined ? (
            error ? null : <LoadingBlock />
          ) : rows.length === 0 ? (
            <EmptyState
              icon={<PersonSearchOutlinedIcon />}
              title="Nema klijenata za odabrane kriterije"
              description="Pokušajte ukloniti neki filtar."
              action={active.length ? <Button onClick={() => apply({})}>Očisti filtre</Button> : undefined}
            />
          ) : (
            <ClientAnalysisTable rows={rows} />
          )}
        </Box>
      </Paper>
    </>
  );
}
