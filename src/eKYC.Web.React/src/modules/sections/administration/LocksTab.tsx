import RefreshIcon from '@mui/icons-material/Refresh';
import { Alert, Box, Button, Typography } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { A_Object_Locks } from '../../../models';
import { deleteObjectLock, getObjectLocks } from '../../../store/adminLocks';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showSuccessMessage } from '../../../store/message';
import { formatDateTime, fullName } from '../../../utils/formatting';
import AppDataGrid from '../../shared/AppDataGrid';
import ConfirmDialog from '../../shared/ConfirmDialog';
import LoadingBlock from '../../shared/LoadingBlock';

/** "Zaključavanje" - held edit-session locks (A_Object_Locks) with an admin force-release. */
export default function LocksTab() {
  const dispatch = useAppDispatch();
  const { locks, error } = useAppSelector((state) => state.adminLocks);
  const [selected, setSelected] = useState<A_Object_Locks | null>(null);
  const [confirmDelete, setConfirmDelete] = useState(false);

  const load = useCallback(() => {
    setSelected(null);
    return dispatch(getObjectLocks());
  }, [dispatch]);

  useEffect(() => {
    load();
  }, [load]);

  const remove = async () => {
    if (!selected) return;
    setConfirmDelete(false);
    try {
      await dispatch(deleteObjectLock(selected.Object_Lock_Id)).unwrap();
      dispatch(showSuccessMessage('Zaključavanje je obrisano.'));
      load();
    } catch {
      // surfaced through the store's error state / network layer
    }
  };

  const columns = useMemo<GridColDef<A_Object_Locks>[]>(
    () => [
      { field: 'Object_Class', headerName: 'Klasa objekta', flex: 1, minWidth: 180 },
      { field: 'Object_Name', headerName: 'Naziv objekta', flex: 1, minWidth: 180 },
      {
        field: 'Lgn_Nm',
        headerName: 'Korisnik',
        flex: 1,
        minWidth: 200,
        valueGetter: (_value, row) => `${fullName(row.Usr_Nm_Fst, row.Usr_Nm_Lst)} (${row.Lgn_Nm ?? ''})`,
      },
      { field: 'Computer_Name', headerName: 'Računalo', width: 160 },
      { field: 'Locked_At', headerName: 'Zaključano', width: 170, valueFormatter: (v: string) => formatDateTime(v) },
    ],
    [],
  );

  return (
    <>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Zapisi o zaključanim slogovima (korisnik trenutno uređuje slog). Ručno brisanje oslobađa zaključavanje — koristiti samo
        kada je sigurno da korisnik više ne uređuje slog (npr. nakon pada sesije).
      </Typography>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      {locks === null ? (
        error ? null : (
          <LoadingBlock />
        )
      ) : locks.length === 0 ? (
        <Alert severity="success">Nema aktivnih zaključavanja.</Alert>
      ) : (
        <AppDataGrid
          rows={locks}
          columns={columns}
          getRowId={(row) => row.Object_Lock_Id}
          onRowClick={(params) => setSelected(params.row)}
          getRowClassName={(params) => (params.row.Object_Lock_Id === selected?.Object_Lock_Id ? 'selected-row' : '')}
          sx={{ '& .selected-row': { backgroundColor: 'action.selected' }, '& .MuiDataGrid-row': { cursor: 'pointer' } }}
        />
      )}
      <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
        <Button variant="outlined" startIcon={<RefreshIcon />} onClick={load}>
          Osvježi prikaz
        </Button>
        <Button variant="outlined" color="error" disabled={!selected} onClick={() => setConfirmDelete(true)}>
          Brisanje zaključavanja
        </Button>
      </Box>

      <ConfirmDialog
        open={confirmDelete}
        title="Brisanje zaključavanja"
        message={`Obrisati zaključavanje za "${selected?.Object_Class ?? ''}" (korisnik ${selected?.Lgn_Nm ?? ''})?`}
        confirmText="Obriši"
        onConfirm={remove}
        onCancel={() => setConfirmDelete(false)}
      />
    </>
  );
}
