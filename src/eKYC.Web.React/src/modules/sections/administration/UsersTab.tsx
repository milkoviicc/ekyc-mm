import PersonAddAlt1OutlinedIcon from '@mui/icons-material/PersonAddAlt1Outlined';
import { Alert, Box, Button, Chip, FormControlLabel, Stack, Switch } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import dayjs from 'dayjs';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { A_Usr, A_Usr_ISRole } from '../../../models';
import {
  assignRole,
  createUser,
  getRoles,
  getUserRoles,
  getUsers,
  revokeRole,
  updateUser,
} from '../../../store/admin';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showSuccessMessage } from '../../../store/message';
import { formatDateTime, fullName } from '../../../utils/formatting';
import AppDataGrid, { clickableRowsSx } from '../../shared/AppDataGrid';
import ConfirmDialog from '../../shared/ConfirmDialog';
import EmptyState from '../../shared/EmptyState';
import LoadingBlock from '../../shared/LoadingBlock';
import UserDrawer, { UserForm } from './UserDrawer';

const isActiveAssignment = (a: A_Usr_ISRole) => !a.Vld_To_Dt || dayjs(a.Vld_To_Dt).isAfter(dayjs());

/** "Korisnici" - maintain users and the roles they hold. Click a row to edit; users are deactivated, never deleted. */
export default function UsersTab() {
  const dispatch = useAppDispatch();
  const users = useAppSelector((state) => state.admin.users);
  const roles = useAppSelector((state) => state.admin.roles);
  const userRoles = useAppSelector((state) => state.admin.userRoles);

  const [showInactive, setShowInactive] = useState(false);
  // undefined = drawer closed, null = creating, A_Usr = editing
  const [editing, setEditing] = useState<A_Usr | null | undefined>(undefined);
  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);
  const [toRevoke, setToRevoke] = useState<A_Usr_ISRole | null>(null);

  const reload = useCallback(() => {
    dispatch(getUsers());
    dispatch(getRoles());
    dispatch(getUserRoles());
  }, [dispatch]);

  useEffect(() => {
    reload();
  }, [reload]);

  const roleName = useCallback((id?: number | null) => roles.items?.find((r) => r.ISRol_Id === id)?.ISRol_Nm ?? `#${id}`, [roles.items]);

  const rolesByUser = useMemo(() => {
    const map = new Map<number, string[]>();
    for (const a of userRoles.items ?? []) {
      if (a.Usr_Id == null || !isActiveAssignment(a)) continue;
      map.set(a.Usr_Id, [...(map.get(a.Usr_Id) ?? []), roleName(a.ISRol_Id)]);
    }
    return map;
  }, [userRoles.items, roleName]);

  const rows = useMemo(() => (users.items ?? []).filter((u) => showInactive || u.Usr_St === 'V'), [users.items, showInactive]);
  const inactiveCount = (users.items ?? []).filter((u) => u.Usr_St !== 'V').length;

  const openUser = (user: A_Usr | null) => {
    setSaveError(null);
    setEditing(user);
  };

  const save = async (form: UserForm) => {
    setSaving(true);
    setSaveError(null);
    try {
      if (editing) {
        await dispatch(updateUser({ ...editing, ...form })).unwrap();
        dispatch(showSuccessMessage('Korisnik je ažuriran.'));
        setEditing(undefined);
      } else {
        const created = await dispatch(createUser(form)).unwrap();
        dispatch(showSuccessMessage('Korisnik je kreiran. Sada mu možete dodijeliti uloge.'));
        // Stay on the new user so roles can be added right away.
        reload();
        setEditing(created);
        return;
      }
      reload();
    } catch (e) {
      setSaveError((e as { message: string }).message);
    } finally {
      setSaving(false);
    }
  };

  const assign = async (roleId: number, from: string | null, to: string | null) => {
    if (!editing) return;
    try {
      await dispatch(assignRole({ Usr_Id: editing.Usr_Id, ISRol_Id: roleId, Vld_From_Dt: from, Vld_To_Dt: to })).unwrap();
      dispatch(showSuccessMessage(`Uloga ${roleName(roleId)} dodijeljena.`));
      setSaveError(null);
      dispatch(getUserRoles());
    } catch (e) {
      setSaveError((e as { message: string }).message);
    }
  };

  const revoke = async () => {
    if (!toRevoke) return;
    const target = toRevoke;
    setToRevoke(null);
    try {
      await dispatch(revokeRole(target.Usr_ISRol_Id)).unwrap();
      dispatch(showSuccessMessage(`Uloga ${roleName(target.ISRol_Id)} ukinuta.`));
      dispatch(getUserRoles());
    } catch (e) {
      setSaveError((e as { message: string }).message);
    }
  };

  const columns = useMemo<GridColDef<A_Usr>[]>(
    () => [
      { field: 'Lgn_Nm', headerName: 'Prijava', width: 150 },
      {
        field: 'Usr_Nm_Fst',
        headerName: 'Ime i prezime',
        flex: 1,
        minWidth: 180,
        valueGetter: (_v, row) => fullName(row.Usr_Nm_Fst, row.Usr_Nm_Lst),
      },
      { field: 'Email', headerName: 'Email', flex: 1, minWidth: 180 },
      {
        field: 'Usr_St',
        headerName: 'Status',
        width: 120,
        renderCell: (params) =>
          params.row.Usr_St === 'V' ? <Chip size="small" color="success" label="Aktivan" /> : <Chip size="small" variant="outlined" label="Neaktivan" />,
      },
      {
        field: 'roles',
        headerName: 'Uloge',
        flex: 1,
        minWidth: 200,
        sortable: false,
        valueGetter: (_v, row) => (rolesByUser.get(row.Usr_Id) ?? []).join(', '),
        renderCell: (params) => (
          <Stack direction="row" spacing={0.5} sx={{ flexWrap: 'wrap', alignItems: 'center' }} useFlexGap>
            {(rolesByUser.get(params.row.Usr_Id) ?? []).map((r) => (
              <Chip key={r} size="small" variant="outlined" color="primary" label={r} />
            ))}
          </Stack>
        ),
      },
      { field: 'Lst_Lgn_Dt', headerName: 'Zadnja prijava', width: 170, valueFormatter: (v: string | null) => formatDateTime(v) },
    ],
    [rolesByUser],
  );

  if (users.error) return <Alert severity="error">{users.error}</Alert>;

  return (
    <>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2, flexWrap: 'wrap' }}>
        <FormControlLabel
          control={<Switch checked={showInactive} onChange={(e) => setShowInactive(e.target.checked)} />}
          label={`Prikaži neaktivne${inactiveCount ? ` (${inactiveCount})` : ''}`}
        />
        <Box sx={{ flex: 1 }} />
        <Button variant="contained" startIcon={<PersonAddAlt1OutlinedIcon />} onClick={() => openUser(null)}>
          Novi korisnik
        </Button>
      </Box>

      {users.items === null ? (
        <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState title="Nema korisnika" description="Promijenite filtar ili dodajte novog korisnika." />
      ) : (
        <AppDataGrid
          rows={rows}
          columns={columns}
          getRowId={(row) => row.Usr_Id}
          getRowHeight={() => 'auto'}
          onRowClick={(params) => openUser(params.row)}
          sx={[clickableRowsSx, { '& .MuiDataGrid-cell': { py: 0.75 } }]}
        />
      )}

      {editing !== undefined && (
        <UserDrawer
          key={editing?.Usr_Id ?? 'new'}
          open
          user={editing}
          roles={roles.items ?? []}
          assignments={editing ? (userRoles.items ?? []).filter((a) => a.Usr_Id === editing.Usr_Id) : []}
          saving={saving}
          error={saveError}
          onClose={() => setEditing(undefined)}
          onSave={save}
          onAssign={assign}
          onRevoke={setToRevoke}
        />
      )}

      <ConfirmDialog
        open={Boolean(toRevoke)}
        title="Ukidanje uloge"
        message={`Ukinuti ulogu ${roleName(toRevoke?.ISRol_Id)} ovom korisniku? Dodjela završava danas, a zapis ostaje u povijesti.`}
        confirmText="Ukini"
        onConfirm={revoke}
        onCancel={() => setToRevoke(null)}
      />
    </>
  );
}
