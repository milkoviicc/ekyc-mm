import { Alert, Box, Paper, Tab, Tabs } from '@mui/material';
import { ReactElement, useState } from 'react';
import PageHeader from '../../shared/PageHeader';
import AccessRightsTab from './AccessRightsTab';
import LocksTab from './LocksTab';

const notImplemented = (text: string) => <Alert severity="info">Nije još implementirano — {text}</Alert>;

/** "Administriranje" tab. */
export default function Administration() {
  const [tab, setTab] = useState(0);

  const tabs: { label: string; content: ReactElement }[] = [
    { label: 'Prava pristupa', content: <AccessRightsTab /> },
    { label: 'Rizici', content: notImplemented('upravljanje kriterijima i razredima rizičnosti.') },
    { label: 'Definicije', content: notImplemented('uređivanje šifarnika (za pregled vidi Reference Data).') },
    { label: 'Zaključavanje', content: <LocksTab /> },
    { label: 'Provjera klijenata', content: <Alert severity="info">Nije još implementirano.</Alert> },
  ];

  return (
    <>
      <PageHeader title="Administriranje" />
      <Paper elevation={1}>
        <Tabs value={tab} onChange={(_, value: number) => setTab(value)} variant="scrollable">
          {tabs.map((t) => (
            <Tab key={t.label} label={t.label} />
          ))}
        </Tabs>
        <Box sx={{ p: 2 }}>{tabs[tab].content}</Box>
      </Paper>
    </>
  );
}
