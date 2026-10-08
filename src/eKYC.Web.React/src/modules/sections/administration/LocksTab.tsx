import CleaningServicesOutlinedIcon from '@mui/icons-material/CleaningServicesOutlined';
import LockOpenOutlinedIcon from '@mui/icons-material/LockOpenOutlined';
import RefreshIcon from '@mui/icons-material/Refresh';
import { Alert, Box, Button, Chip, FormControlLabel, Switch, Typography } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { A_Object_Locks, ObjectLockSettings } from '../../../models';
import { deleteObjectLock, getObjectLocks } from '../../../store/adminLocks';
import { getLockSettings } from '../../../store/editLocks';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showErrorMessage, showSuccessMessage } from '../../../store/message';
import { formatDateTime, formatDuration, fullName } from '../../../utils/formatting';
import AppDataGrid, { clickableRowsSx } from '../../shared/AppDataGrid';
import ConfirmDialog from '../../shared/ConfirmDialog';
import EmptyState from '../../shared/EmptyState';
import LoadingBlock from '../../shared/LoadingBlock';

const AUTO_REFRESH_MS = 15_000;
const DEFAULT_TTL_MINUTES = 30;

/**
 * "Zaključavanje" - edit-session locks currently held (A_Object_Locks). The list refreshes itself every 15 seconds while this
 * tab is open. A lock nobody has refreshed for the configured time is "zastarjelo" (abandoned) and can be released in bulk;
 * releasing a live lock can make its holder lose unsaved work, so that one asks for confirmation naming the holder.
 */
export default function LocksTab() {
  const dispatch = useAppDispatch();
  const { locks, error } = useAppSelector((state) => state.adminLocks);
  const [settings, setSettings] = useState<ObjectLockSettings | null>(null);
  const [selected, setSelected] = useState<A_Object_Locks | null>(null);
  const [autoRefresh, setAutoRefresh] = useState(true);
  const [confirmOne, setConfirmOne] = useState(false);
  const [confirmStale, setConfirmStale] = useState(false);

  const ttlSeconds = (settings?.TtlMinutes ?? DEFAULT_TTL_MINUTES) * 60;
  const isStale = useCallback((lock: A_Object_Locks) => (lock.Age_Seconds ?? 0) >= ttlSeconds, [ttlSeconds]);

  const load = useCallback(() => dispatch(getObjectLocks()), [dispatch]);

  useEffect(() => {
    load();
    dispatch(getLockSettings())
      .unwrap()
      .then(setSettings)
      .catch(() => undefined); // the default TTL is used when the settings cannot be read
  }, [load, dispatch]);

  useEffect(() => {
    if (!autoRefresh) return;
    const timer = setInterval(load, AUTO_REFRESH_MS);
    return () => clearInterval(timer);
  }, [autoRefresh, load]);

  // Keep the selection pointing at a lock that still exists after a refresh.
  const selectedLock = selected ? (locks?.find((l) => l.Object_Lock_Id === selected.Object_Lock_Id) ?? null) : null;
  const staleLocks = useMemo(() => (locks ?? []).filter(isStale), [locks, isStale]);

  const releaseSelected = async () => {
    if (!selectedLock) return;
    setConfirmOne(false);
    try {
      await dispatch(deleteObjectLock(selectedLock.Object_Lock_Id)).unwrap();
      dispatch(showSuccessMessage('Zaključavanje je oslobođeno.'));
      setSelected(null);
    } catch (e) {
      dispatch(showErrorMessage((e as { message: string }).message));
    }
    load();
  };

  const releaseStale = async () => {
    setConfirmStale(false);
    let released = 0;
    for (const lock of staleLocks) {
      try {
        await dispatch(deleteObjectLock(lock.Object_Lock_Id)).unwrap();
        released++;
      } catch (e) {
        dispatch(showErrorMessage((e as { message: string }).message));
        break;
      }
    }
    if (released > 0) dispatch(showSuccessMessage(`Oslobođeno zastarjelih zaključavanja: ${released}.`));
    load();
  };

  const columns = useMemo<GridColDef<A_Object_Locks>[]>(
    () => [
      {
        field: 'Object_Class',
        headerName: 'Zapis',
        flex: 1,
        minWidth: 240,
        renderCell: (params) => (
          <Box sx={{ minWidth: 0 }}>
            <Typography variant="body2" noWrap sx={{ fontWeight: 600, lineHeight: 1.3 }}>
              {params.row.Object_Name || `#${params.row.Object_Id}`}
            </Typography>
            <Typography variant="caption" color="text.secondary" noWrap component="div">
              {params.row.Object_Class} #{params.row.Object_Id}
            </Typography>
          </Box>
        ),
      },
      {
        field: 'Lgn_Nm',
        headerName: 'Korisnik',
        flex: 1,
        minWidth: 200,
        valueGetter: (_v, row) => `${fullName(row.Usr_Nm_Fst, row.Usr_Nm_Lst)} (${row.Lgn_Nm ?? ''})`,
      },
      { field: 'Computer_Name', headerName: 'Računalo', width: 150 },
      { field: 'Locked_At', headerName: 'Zadnja aktivnost', width: 170, valueFormatter: (v: string) => formatDateTime(v) },
      {
        field: 'Age_Seconds',
        headerName: 'Traje',
        width: 190,
        type: 'number',
        headerAlign: 'left',
        align: 'left',
        renderCell: (params) => (
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <span>{formatDuration(params.row.Age_Seconds)}</span>
            {isStale(params.row) && <Chip size="small" color="warning" label="Zastarjelo" />}
          </Box>
        ),
      },
    ],
    [isStale],
  );

  return (
    <>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Zapisi koje korisnici trenutno uređuju. Zaključavanje se produljuje dok je uređivanje otvoreno; ako se {settings?.TtlMinutes ?? DEFAULT_TTL_MINUTES} minuta ne
        obnovi, smatra se zastarjelim (npr. nakon pada preglednika) i drugi korisnik ga može preuzeti.
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2, flexWrap: 'wrap' }}>
        <Button variant="outlined" color="inherit" sx={{ borderColor: 'divider' }} startIcon={<RefreshIcon />} onClick={load}>
          Osvježi
        </Button>
        <FormControlLabel
          control={<Switch checked={autoRefresh} onChange={(e) => setAutoRefresh(e.target.checked)} />}
          label="Automatski svakih 15 s"
        />
        <Box sx={{ flex: 1 }} />
        <Button
          variant="outlined"
          color="warning"
          startIcon={<CleaningServicesOutlinedIcon />}
          disabled={staleLocks.length === 0}
          onClick={() => setConfirmStale(true)}
        >
          Oslobodi zastarjela ({staleLocks.length})
        </Button>
        <Button variant="outlined" color="error" disabled={!selectedLock} onClick={() => setConfirmOne(true)}>
          Oslobodi odabrano
        </Button>
      </Box>

      {locks === null ? (
        error ? null : <LoadingBlock rows={3} />
      ) : locks.length === 0 ? (
        <EmptyState icon={<LockOpenOutlinedIcon />} title="Nema aktivnih zaključavanja" description="Nitko trenutno ne uređuje nijedan slog." />
      ) : (
        <AppDataGrid
          rows={locks}
          columns={columns}
          getRowId={(row) => row.Object_Lock_Id}
          getRowHeight={() => 56}
          onRowClick={(params) => setSelected(params.row)}
          getRowClassName={(params) => (params.row.Object_Lock_Id === selectedLock?.Object_Lock_Id ? 'selected-row' : '')}
          sx={clickableRowsSx}
        />
      )}

      <ConfirmDialog
        open={confirmOne}
        title="Oslobađanje zaključavanja"
        message={
          selectedLock
            ? isStale(selectedLock)
              ? `Osloboditi zastarjelo zaključavanje za „${selectedLock.Object_Name || selectedLock.Object_Class}“?`
              : `Korisnik ${selectedLock.Lgn_Nm} je ${formatDuration(selectedLock.Age_Seconds)} aktivan na zapisu „${selectedLock.Object_Name || selectedLock.Object_Class}“. Oslobađanjem može izgubiti nespremljene izmjene. Osloboditi?`
            : ''
        }
        confirmText="Oslobodi"
        onConfirm={releaseSelected}
        onCancel={() => setConfirmOne(false)}
      />
      <ConfirmDialog
        open={confirmStale}
        title="Oslobađanje zastarjelih zaključavanja"
        message={`Osloboditi ${staleLocks.length} zaključavanja koja nisu obnovljena dulje od ${settings?.TtlMinutes ?? DEFAULT_TTL_MINUTES} minuta?`}
        confirmText="Oslobodi"
        onConfirm={releaseStale}
        onCancel={() => setConfirmStale(false)}
      />
    </>
  );
}
