import ConstructionOutlinedIcon from '@mui/icons-material/ConstructionOutlined';
import { Box, Paper, Tab, Tabs } from '@mui/material';
import { ReactElement } from 'react';
import { useUrlTab } from '../../../hooks/useUrlTab';
import EmptyState from '../../shared/EmptyState';
import PageHeader from '../../shared/PageHeader';
import AccessRightsTab from './AccessRightsTab';
import AuditTab from './AuditTab';
import LocksTab from './LocksTab';
import ParametersTab from './ParametersTab';

const notImplemented = (description: string) => (
  <EmptyState icon={<ConstructionOutlinedIcon />} title="Još nije implementirano" description={description} />
);

const TABS: { id: string; label: string; content: ReactElement }[] = [
  { id: 'prava', label: 'Prava pristupa', content: <AccessRightsTab /> },
  { id: 'postavke', label: 'Postavke', content: <ParametersTab /> },
  { id: 'zakljucavanje', label: 'Zaključavanje', content: <LocksTab /> },
  { id: 'dnevnik', label: 'Dnevnik promjena', content: <AuditTab /> },
  { id: 'rizici', label: 'Rizici', content: notImplemented('Upravljanje kriterijima i razredima rizičnosti.') },
  { id: 'definicije', label: 'Definicije', content: notImplemented('Uređivanje šifarnika. Za pregled šifarnika vidi Reference Data.') },
  { id: 'provjera', label: 'Provjera klijenata', content: notImplemented('Provjera klijenata.') },
];
const TAB_IDS = TABS.map((t) => t.id);

/** "Administriranje" tab. The selected tab is kept in the URL (?tab=zakljucavanje). */
export default function Administration() {
  const [tab, setTab] = useUrlTab(TAB_IDS);

  return (
    <>
      <PageHeader title="Administriranje" subtitle="Korisnici i prava, postavke sustava, zaključani slogovi i dnevnik promjena." />
      <Paper variant="outlined" sx={{ overflow: 'hidden' }}>
        <Tabs value={tab} onChange={(_, value: number) => setTab(value)} variant="scrollable" sx={{ px: 1, borderBottom: 1, borderColor: 'divider' }}>
          {TABS.map((t) => (
            <Tab key={t.id} label={t.label} />
          ))}
        </Tabs>
        <Box sx={{ p: 2 }}>{TABS[tab].content}</Box>
      </Paper>
    </>
  );
}
