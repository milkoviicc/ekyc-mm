import AddIcon from '@mui/icons-material/Add';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Typography } from '@mui/material';
import { GridActionsCellItem, GridColDef } from '@mui/x-data-grid';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { A_Apl_Prmtr } from '../../../models';
import { createParameter, getParameters, updateParameter } from '../../../store/admin';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showSuccessMessage } from '../../../store/message';
import AppDataGrid, { clickableRowsSx } from '../../shared/AppDataGrid';
import EmptyState from '../../shared/EmptyState';
import LoadingBlock from '../../shared/LoadingBlock';

type Editing = { parameter: A_Apl_Prmtr | null };

type DialogProps = {
  /** The parameter being edited, or null to add a new one. Remounted per open (see key below). */
  parameter: A_Apl_Prmtr | null;
  saving: boolean;
  error: string | null;
  onCancel: () => void;
  onSave: (code: string, value: string, description: string) => void;
};

function ParameterDialog({ parameter, saving, error, onCancel, onSave }: DialogProps) {
  const [code, setCode] = useState(parameter?.Prmtr_Cd ?? '');
  const [value, setValue] = useState(parameter?.Prmtr_Val ?? '');
  const [description, setDescription] = useState(parameter?.Prmtr_Dspn ?? '');
  const [submitted, setSubmitted] = useState(false);

  const codeMissing = code.trim().length === 0;

  return (
    <Dialog open onClose={onCancel} maxWidth="sm" fullWidth>
      <DialogTitle>{parameter ? `Parametar ${parameter.Prmtr_Cd}` : 'Novi parametar'}</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            label="Šifra"
            required
            value={code}
            disabled={parameter !== null}
            onChange={(e) => setCode(e.target.value)}
            error={submitted && codeMissing}
            helperText={parameter ? 'Šifra se ne može mijenjati.' : submitted && codeMissing ? 'Šifra je obvezna.' : 'Jedinstvena, najviše 50 znakova.'}
            slotProps={{ htmlInput: { maxLength: 50 } }}
          />
          <TextField label="Vrijednost" multiline minRows={2} value={value} onChange={(e) => setValue(e.target.value)} />
          <TextField label="Opis" multiline minRows={2} value={description} onChange={(e) => setDescription(e.target.value)} />
          <Typography variant="caption" color="text.secondary">
            Promjena se odmah primjenjuje i bilježi u dnevniku promjena sa starom i novom vrijednošću.
          </Typography>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Odustanak</Button>
        <Button
          variant="contained"
          disabled={saving}
          onClick={() => {
            setSubmitted(true);
            if (!codeMissing) onSave(code.trim(), value, description);
          }}
        >
          Spremi
        </Button>
      </DialogActions>
    </Dialog>
  );
}

/** "Postavke" - application parameters (A_Apl_Prmtr). Values often hold paths and URLs that differ per environment, so edit with care. */
export default function ParametersTab() {
  const dispatch = useAppDispatch();
  const parameters = useAppSelector((state) => state.admin.parameters);
  const [editing, setEditing] = useState<Editing | null>(null);
  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);

  const reload = useCallback(() => dispatch(getParameters()), [dispatch]);

  useEffect(() => {
    reload();
  }, [reload]);

  const open = (parameter: A_Apl_Prmtr | null) => {
    setSaveError(null);
    setEditing({ parameter });
  };

  const save = async (code: string, value: string, description: string) => {
    setSaving(true);
    setSaveError(null);
    try {
      if (editing?.parameter) {
        await dispatch(updateParameter({ ...editing.parameter, Prmtr_Val: value, Prmtr_Dspn: description })).unwrap();
        dispatch(showSuccessMessage('Parametar je ažuriran.'));
      } else {
        await dispatch(createParameter({ Prmtr_Cd: code, Prmtr_Val: value, Prmtr_Dspn: description })).unwrap();
        dispatch(showSuccessMessage('Parametar je dodan.'));
      }
      setEditing(null);
      reload();
    } catch (e) {
      setSaveError((e as { message: string }).message);
    } finally {
      setSaving(false);
    }
  };

  const columns = useMemo<GridColDef<A_Apl_Prmtr>[]>(
    () => [
      { field: 'Prmtr_Cd', headerName: 'Šifra', width: 260 },
      { field: 'Prmtr_Val', headerName: 'Vrijednost', flex: 1.5, minWidth: 260 },
      { field: 'Prmtr_Dspn', headerName: 'Opis', flex: 1, minWidth: 220 },
      {
        field: 'actions',
        type: 'actions',
        headerName: '',
        width: 64,
        getActions: ({ row }) => [<GridActionsCellItem key="edit" icon={<EditOutlinedIcon />} label="Uredi parametar" onClick={() => open(row)} />],
      },
    ],
    [],
  );

  if (parameters.error) return <Alert severity="error">{parameters.error}</Alert>;

  return (
    <>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2, flexWrap: 'wrap' }}>
        <Typography variant="body2" color="text.secondary" sx={{ flex: 1, minWidth: 260 }}>
          Postavke aplikacije. Neke vrijednosti (adrese, putanje) vrijede samo za određeno okruženje.
        </Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => open(null)}>
          Novi parametar
        </Button>
      </Box>

      {parameters.items === null ? (
        <LoadingBlock rows={4} />
      ) : parameters.items.length === 0 ? (
        <EmptyState title="Nema parametara" description="Dodajte prvi parametar." />
      ) : (
        <AppDataGrid
          rows={parameters.items}
          columns={columns}
          getRowId={(p) => p.Apl_Id}
          getRowHeight={() => 'auto'}
          onRowClick={(params, event) => {
            if ((event.target as HTMLElement).closest('.MuiDataGrid-actionsCell')) return;
            open(params.row);
          }}
          sx={[clickableRowsSx, { '& .MuiDataGrid-cell': { py: 1, wordBreak: 'break-word' } }]}
        />
      )}

      {editing && (
        <ParameterDialog
          key={String(editing.parameter?.Apl_Id ?? 'new')}
          parameter={editing.parameter}
          saving={saving}
          error={saveError}
          onCancel={() => setEditing(null)}
          onSave={save}
        />
      )}
    </>
  );
}
