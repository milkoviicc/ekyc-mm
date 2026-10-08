import RefreshIcon from '@mui/icons-material/Refresh';
import SearchOffOutlinedIcon from '@mui/icons-material/SearchOffOutlined';
import { Alert, Box, Button, Paper, Tab, Tabs, Typography } from '@mui/material';
import { ReactNode, useEffect } from 'react';
import { useClientTypes, useRiskEstimates } from '../../../hooks/useReferenceData';
import { useUrlTab } from '../../../hooks/useUrlTab';
import { ReportKey, ReportKeys, ReportRiskFilter } from '../../../models';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { getRiskReport } from '../../../store/reports';
import EmptyState from '../../shared/EmptyState';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import ReportRiskFilterForm from './ReportRiskFilterForm';
import ReportRiskTable from './ReportRiskTable';

const TAB_IDS = ['razdoblje', 'dan', 'visoki-rizik'] as const;

/** "Izvješća" tab - three risk reports. The active tab is kept in the URL (?tab=dan). */
export default function Reports() {
  const dispatch = useAppDispatch();
  const { rowsByReport, pendingReports, error } = useAppSelector((state) => state.reports);
  const clientTypes = useClientTypes();
  const riskEstimates = useRiskEstimates();
  const [tab, setTab] = useUrlTab(TAB_IDS);

  // All three reports are loaded up front, like the Blazor page does.
  useEffect(() => {
    dispatch(getRiskReport({ reportKey: ReportKeys.RiskPeriod }));
    dispatch(getRiskReport({ reportKey: ReportKeys.RiskDay }));
    dispatch(getRiskReport({ reportKey: ReportKeys.HighRiskReprocess }));
  }, [dispatch]);

  const load = (reportKey: ReportKey, filter?: ReportRiskFilter) => dispatch(getRiskReport({ reportKey, filter }));

  const result = (reportKey: ReportKey): ReactNode => {
    const rows = rowsByReport[reportKey];
    if (rows === undefined) return pendingReports.includes(reportKey) || !error ? <LoadingBlock /> : null;
    if (rows.length === 0) {
      return <EmptyState icon={<SearchOffOutlinedIcon />} title="Nema rezultata za odabrane kriterije" description="Pokušajte proširiti razdoblje ili ukloniti filtre." />;
    }
    return <ReportRiskTable rows={rows} />;
  };

  const countOf = (reportKey: ReportKey) => rowsByReport[reportKey]?.length;
  const tabLabel = (label: string, reportKey: ReportKey) => (
    <Box component="span" sx={{ display: 'inline-flex', alignItems: 'center', gap: 1 }}>
      {label}
      {countOf(reportKey) !== undefined && (
        <Box component="span" sx={{ px: 0.75, borderRadius: 1, bgcolor: 'action.hover', fontSize: 12 }}>
          {countOf(reportKey)}
        </Box>
      )}
    </Box>
  );

  return (
    <>
      <PageHeader title="Izvješća" subtitle="Izvješća o rizičnosti klijenata. Tablicu možete izvesti u CSV putem ikone za preuzimanje." />
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      <Paper variant="outlined" sx={{ overflow: 'hidden' }}>
        <Tabs value={tab} onChange={(_, value: number) => setTab(value)} variant="scrollable" sx={{ px: 1, borderBottom: 1, borderColor: 'divider' }}>
          <Tab label={tabLabel('Rizičnost - razdoblje', ReportKeys.RiskPeriod)} />
          <Tab label={tabLabel('Rizičnost - na dan', ReportKeys.RiskDay)} />
          <Tab label={tabLabel('Ponovna obrada visoko rizičnih', ReportKeys.HighRiskReprocess)} />
        </Tabs>
        <Box sx={{ p: 2 }}>
          {tab === 0 && (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Pregled stranaka po razdoblju od - do i po vrstama rizičnosti. Prikazani su samo aktivni i odbijeni klijenti.
              </Typography>
              <ReportRiskFilterForm clientTypes={clientTypes} riskEstimates={riskEstimates} onApply={(f) => load(ReportKeys.RiskPeriod, f)} />
              {result(ReportKeys.RiskPeriod)}
            </>
          )}
          {tab === 1 && (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Pregled stranaka na dan po vrstama rizičnosti. Prikazani su samo aktivni i odbijeni klijenti.
              </Typography>
              <ReportRiskFilterForm clientTypes={clientTypes} riskEstimates={riskEstimates} onApply={(f) => load(ReportKeys.RiskDay, f)} />
              {result(ReportKeys.RiskDay)}
            </>
          )}
          {tab === 2 && (
            <>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2, flexWrap: 'wrap' }}>
                <Typography variant="body2" color="text.secondary" sx={{ flex: 1, minWidth: 260 }}>
                  Visokorizični klijenti (7+ bodova) koji čekaju ponovnu obradu ili ponovnu dubinsku analizu — status „Upitnik kreiran“ ili „Izmjena“. Kriteriji su fiksni.
                </Typography>
                <Button variant="outlined" color="inherit" startIcon={<RefreshIcon />} sx={{ borderColor: 'divider' }} onClick={() => load(ReportKeys.HighRiskReprocess)}>
                  Osvježi
                </Button>
              </Box>
              {result(ReportKeys.HighRiskReprocess)}
            </>
          )}
        </Box>
      </Paper>
    </>
  );
}
