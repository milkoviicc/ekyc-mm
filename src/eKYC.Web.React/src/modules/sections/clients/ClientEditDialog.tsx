import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
} from '@mui/material';
import { useState } from 'react';
import { ClientProcessingStatus, CL_Clnt, RiskEstimate } from '../../../models';
import { SelectField } from '../../shared/SelectField';

type Props = {
  open: boolean;
  /** Copy of the row being edited, so cancelling leaves the grid untouched. */
  client: CL_Clnt;
  processingStatuses: ClientProcessingStatus[] | null;
  riskEstimates: RiskEstimate[] | null;
  saving: boolean;
  onCancel: () => void;
  onSave: (client: CL_Clnt) => void;
};

const PEP_OPTIONS = ['DA', 'NE', 'NE ODREĐUJE SE'];

/**
 * Edits the core CL_Clnt fields (processing status, risk, PEP, watchlist, remark). The type-specific detail
 * (name/OIB/address) is a separate, larger piece not built yet. Client type is read-only since changing it
 * after creation would orphan the type-specific record.
 */
export default function ClientEditDialog({ open, client, processingStatuses, riskEstimates, saving, onCancel, onSave }: Props) {
  const [processingSt, setProcessingSt] = useState<string | null>(client.Clnt_Prcsng_St || null);
  const [riskEstId, setRiskEstId] = useState<number | null>(client.RskEst_Id ?? null);
  const [riskPoints, setRiskPoints] = useState<string>(client.Rsk_Pnts?.toString() ?? '');
  const [pep, setPep] = useState<string>(client.PEP_Ind ?? 'NE ODREĐUJE SE');
  const [watchlist, setWatchlist] = useState<string>(client.WtchLst_Ind || 'N');
  const [remark, setRemark] = useState(client.Rmrk ?? '');

  const accept = () =>
    onSave({
      ...client,
      Clnt_Prcsng_St: processingSt ?? client.Clnt_Prcsng_St,
      RskEst_Id: riskEstId,
      Rsk_Pnts: riskPoints.trim() === '' ? null : Number(riskPoints),
      PEP_Ind: pep,
      WtchLst_Ind: watchlist,
      Rmrk: remark.trim() === '' ? null : remark,
    });

  return (
    <Dialog open={open} onClose={onCancel} maxWidth="sm" fullWidth>
      <DialogTitle>Klijent #{client.Clnt_Id}</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
          <TextField size="small" label="Tip klijenta" value={client.Clnt_Typ_Cd} slotProps={{ input: { readOnly: true } }} />
          <SelectField
            label="Status obrade"
            clearable={false}
            value={processingSt}
            onChange={setProcessingSt}
            options={(processingStatuses ?? []).map((ps) => ({ value: ps.ClntPrcsStCd, label: ps.Status ?? ps.ClntPrcsStCd }))}
          />
          <SelectField
            label="Procjena rizika"
            value={riskEstId}
            onChange={setRiskEstId}
            options={(riskEstimates ?? []).map((re) => ({ value: re.RskEstId, label: re.RiskLevel ?? String(re.RskEstId) }))}
          />
          <TextField
            size="small"
            type="number"
            label="Bodovi rizika"
            value={riskPoints}
            onChange={(e) => setRiskPoints(e.target.value)}
            slotProps={{ htmlInput: { min: 0 } }}
          />
          <SelectField
            label="PEP"
            clearable={false}
            value={pep}
            onChange={(v) => setPep(v ?? 'NE ODREĐUJE SE')}
            options={PEP_OPTIONS.map((p) => ({ value: p, label: p }))}
          />
          <SelectField
            label="Watchlist"
            clearable={false}
            value={watchlist}
            onChange={(v) => setWatchlist(v ?? 'N')}
            options={[
              { value: 'N', label: 'NE' },
              { value: 'D', label: 'DA' },
            ]}
          />
          <TextField size="small" label="Napomena" multiline minRows={3} value={remark} onChange={(e) => setRemark(e.target.value)} />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Odustanak</Button>
        <Button variant="contained" onClick={accept} disabled={saving}>
          Spremi
        </Button>
      </DialogActions>
    </Dialog>
  );
}
