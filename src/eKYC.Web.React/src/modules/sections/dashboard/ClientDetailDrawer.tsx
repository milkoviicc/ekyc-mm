import CloseIcon from '@mui/icons-material/Close';
import SyncProblemIcon from '@mui/icons-material/SyncProblem';
import { Alert, Box, Divider, Drawer, IconButton, Stack, Typography } from '@mui/material';
import { ReactNode } from 'react';
import { DashboardClientRow } from '../../../models';
import { formatDateTime } from '../../../utils/formatting';
import { isMasterDataChanged } from '../../../utils/dashboardRowStyling';
import { FlagChip, RiskChip, StatusChip } from '../../shared/StatusChips';

type Props = {
  row: DashboardClientRow | null;
  onClose: () => void;
};

function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <Box>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="body2" component="div" sx={{ fontWeight: 500, mt: 0.25, wordBreak: 'break-word' }}>
        {children || '—'}
      </Typography>
    </Box>
  );
}

/** Side panel with the details of the clicked client - keeps the list in view instead of navigating away. */
export default function ClientDetailDrawer({ row, onClose }: Props) {
  return (
    <Drawer
      anchor="right"
      open={Boolean(row)}
      onClose={onClose}
      slotProps={{ paper: { sx: { width: { xs: '100%', sm: 420 }, p: 0 } } }}
    >
      {row && (
        <>
          <Stack direction="row" sx={{ p: 2.5, gap: 1, alignItems: "flex-start" }}>
            <Box sx={{ flex: 1, minWidth: 0 }}>
              <Typography variant="overline" color="text.secondary">
                {row.VrstaKlijenta}
              </Typography>
              <Typography variant="h5" sx={{ wordBreak: 'break-word' }}>
                {row.ClntNm}
              </Typography>
            </Box>
            <IconButton onClick={onClose} aria-label="Zatvori">
              <CloseIcon />
            </IconButton>
          </Stack>
          <Divider />
          <Stack spacing={2.5} sx={{ p: 2.5 }}>
            {isMasterDataChanged(row) && (
              <Alert severity="warning" icon={<SyncProblemIcon />}>
                Matični podaci se razlikuju od HBOR izvora.
              </Alert>
            )}
            {row.MustModify && <Alert severity="error">Klijent zahtijeva izmjenu.</Alert>}

            <Stack direction="row" spacing={1} useFlexGap sx={{ flexWrap: "wrap" }}>
              <StatusChip code={row.ClntPrcsngSt} label={row.Status} />
              <RiskChip points={row.RskPnts} />
              <FlagChip active={row.PepInd === 'DA'} label="PEP" />
            </Stack>

            <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 2 }}>
              <Field label="OIB">{row.Oib}</Field>
              <Field label="HBOR ID">{row.HborId}</Field>
              <Field label="Bodovi rizika">{row.RskPnts}</Field>
              <Field label="PEP">{row.PepInd}</Field>
              <Field label="Watchlist">{row.WtchLstInd}</Field>
              <Field label="Zadnja izmjena">{formatDateTime(row.MdfDt)}</Field>
              <Field label="Izmijenio">{row.ModifiedByName}</Field>
            </Box>
          </Stack>
        </>
      )}
    </Drawer>
  );
}
