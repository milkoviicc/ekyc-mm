import { Alert, Box, Button, Paper, Tab, Tabs, Typography } from '@mui/material';
import { useEffect, useState } from 'react';
import { useClientTypes, useRiskEstimates } from '../../../hooks/useReferenceData';
import { ReportKey, ReportKeys, ReportRiskFilter } from '../../../models';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { getRiskReport } from '../../../store/reports';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import ReportRiskFilterForm from './ReportRiskFilterForm';
import ReportRiskTable from './ReportRiskTable';

/** "Izvješća" tab - three risk reports. */
export default function Reports() {
  const dispatch = useAppDispatch();
  const { rowsByReport, pendingReports, error } = useAppSelector((state) => state.reports);
  const clientTypes = useClientTypes();
  const riskEstimates = useRiskEstimates();

  const [tab, setTab] = useState(0);
  const [filterPeriod, setFilterPeriod] = useState<ReportRiskFilter>({});
  const [filterDay, setFilterDay] = useState<ReportRiskFilter>({});

  // All three reports are loaded up front, like the Blazor page does.
  useEffect(() => {
    dispatch(getRiskReport({ reportKey: ReportKeys.RiskPeriod }));
    dispatch(getRiskReport({ reportKey: ReportKeys.RiskDay }));
    dispatch(getRiskReport({ reportKey: ReportKeys.HighRiskReprocess }));
  }, [dispatch]);

  const load = (reportKey: ReportKey, filter?: ReportRiskFilter) => dispatch(getRiskReport({ reportKey, filter }));

  const result = (reportKey: ReportKey) => {
    const rows = rowsByReport[reportKey];
    if (rows === undefined) return pendingReports.includes(reportKey) || !error ? <LoadingBlock /> : null;
    if (rows.length === 0) return <Alert severity="info">Nema rezultata za odabrane kriterije.</Alert>;
    return <ReportRiskTable rows={rows} />;
  };

  return (
    <>
      <PageHeader title="Izvješća" />
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      <Paper elevation={1}>
        <Tabs value={tab} onChange={(_, value: number) => setTab(value)} variant="scrollable">
          <Tab label="Vrste rizičnosti - za razdoblje" />
          <Tab label="Vrste rizičnosti - za dan" />
          <Tab label="Ponovna obrada visoko rizičnih" />
        </Tabs>
        <Box sx={{ p: 2 }}>
          {tab === 0 && (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Pregled stranaka po razdoblju od - do i po vrstama rizičnosti. Prikazani su samo aktivni i odbijeni klijenti.
              </Typography>
              <ReportRiskFilterForm
                filter={filterPeriod}
                onChange={setFilterPeriod}
                clientTypes={clientTypes}
                riskEstimates={riskEstimates}
                onApply={(f) => load(ReportKeys.RiskPeriod, f)}
              />
              {result(ReportKeys.RiskPeriod)}
            </>
          )}
          {tab === 1 && (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Pregled stranaka na dan po vrstama rizičnosti. Prikazani su samo aktivni i odbijeni klijenti.
              </Typography>
              <ReportRiskFilterForm
                filter={filterDay}
                onChange={setFilterDay}
                clientTypes={clientTypes}
                riskEstimates={riskEstimates}
                onApply={(f) => load(ReportKeys.RiskDay, f)}
              />
              {result(ReportKeys.RiskDay)}
            </>
          )}
          {tab === 2 && (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Visokorizični klijenti (7+ bodova) koji čekaju ponovnu obradu ili ponovnu dubinsku analizu — status
                &quot;Upitnik kreiran&quot; ili &quot;Izmjena&quot;. Kriteriji su fiksni.
              </Typography>
              <Button variant="contained" sx={{ mb: 2 }} onClick={() => load(ReportKeys.HighRiskReprocess)}>
                Osvježi prikaz
              </Button>
              {result(ReportKeys.HighRiskReprocess)}
            </>
          )}
        </Box>
      </Paper>
    </>
  );
}
