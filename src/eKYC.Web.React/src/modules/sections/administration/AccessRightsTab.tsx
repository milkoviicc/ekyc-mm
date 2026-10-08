import { Alert, Box, Chip, Stack, Tab, Tabs } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useEffect, useMemo } from 'react';
import { useUrlTab } from '../../../hooks/useUrlTab';
import { A_ISRole, A_Objct } from '../../../models';
import { getObjects, getObjectTypes, getRoleObjects, getRoles, getUserRoles } from '../../../store/admin';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import AppDataGrid from '../../shared/AppDataGrid';
import LoadingBlock from '../../shared/LoadingBlock';
import PermissionMatrixTab from './PermissionMatrixTab';
import UsersTab from './UsersTab';

const TAB_IDS = ['korisnici', 'prava', 'uloge', 'objekti'] as const;

function RolesView() {
  const dispatch = useAppDispatch();
  const roles = useAppSelector((state) => state.admin.roles);
  const userRoles = useAppSelector((state) => state.admin.userRoles);
  const grants = useAppSelector((state) => state.admin.roleObjects);

  useEffect(() => {
    dispatch(getRoles());
    dispatch(getUserRoles());
    dispatch(getRoleObjects());
  }, [dispatch]);

  const columns = useMemo<GridColDef<A_ISRole>[]>(() => {
    const now = Date.now();
    return [
      { field: 'ISRol_Nm', headerName: 'Uloga', width: 180 },
      { field: 'Apl_Cd', headerName: 'Šifra aplikacije', width: 180 },
      {
        field: 'ISRol_St',
        headerName: 'Status',
        width: 120,
        renderCell: (params) =>
          params.row.ISRol_St === 'V' ? (
            <Chip size="small" color="success" label="Aktivna" />
          ) : (
            <Chip size="small" variant="outlined" label={params.row.ISRol_St ?? '—'} />
          ),
      },
      {
        field: 'users',
        headerName: 'Aktivnih korisnika',
        type: 'number',
        width: 170,
        valueGetter: (_v, row) =>
          new Set(
            (userRoles.items ?? [])
              .filter((a) => a.ISRol_Id === row.ISRol_Id && (!a.Vld_To_Dt || new Date(a.Vld_To_Dt).getTime() > now))
              .map((a) => a.Usr_Id),
          ).size,
      },
      {
        field: 'screens',
        headerName: 'Ekrana',
        type: 'number',
        width: 110,
        valueGetter: (_v, row) => (grants.items ?? []).filter((g) => g.ISRol_Id === row.ISRol_Id).length,
      },
    ];
  }, [userRoles.items, grants.items]);

  if (roles.error) return <Alert severity="error">{roles.error}</Alert>;
  if (roles.items === null) return <LoadingBlock rows={4} />;
  return <AppDataGrid rows={roles.items} columns={columns} getRowId={(r) => r.ISRol_Id} />;
}

function ObjectsView() {
  const dispatch = useAppDispatch();
  const objects = useAppSelector((state) => state.admin.objects);
  const types = useAppSelector((state) => state.admin.objectTypes);
  const roles = useAppSelector((state) => state.admin.roles);
  const grants = useAppSelector((state) => state.admin.roleObjects);

  useEffect(() => {
    dispatch(getObjects());
    dispatch(getObjectTypes());
    dispatch(getRoles());
    dispatch(getRoleObjects());
  }, [dispatch]);

  // Roles that may open each object (from A_ISRole_Objct), valid roles only - same idea as the "Uloge" column of Korisnici.
  const rolesByObject = useMemo(() => {
    const map = new Map<number, string[]>();
    for (const grant of grants.items ?? []) {
      if (grant.Objct_Id == null) continue;
      const role = roles.items?.find((r) => r.ISRol_Id === grant.ISRol_Id);
      if (!role || role.ISRol_St !== 'V' || !role.ISRol_Nm) continue;
      const names = map.get(grant.Objct_Id) ?? [];
      if (!names.includes(role.ISRol_Nm)) map.set(grant.Objct_Id, [...names, role.ISRol_Nm]);
    }
    return map;
  }, [grants.items, roles.items]);

  const columns = useMemo<GridColDef<A_Objct>[]>(
    () => [
      { field: 'Objct_Dspn', headerName: 'Naziv', flex: 1, minWidth: 200 },
      { field: 'Objct_Nm', headerName: 'Oznaka', width: 180 },
      {
        field: 'Objct_Typ_Id',
        headerName: 'Vrsta',
        width: 140,
        valueGetter: (_v, row) => types.items?.find((t) => t.Objct_Typ_Id === row.Objct_Typ_Id)?.Typ_Nm ?? row.Objct_Typ_Id,
      },
      { field: 'Asmbly_Cd', headerName: 'Šifra', width: 160 },
      {
        field: 'roles',
        headerName: 'Uloge',
        flex: 1,
        minWidth: 240,
        sortable: false,
        valueGetter: (_v, row) => (rolesByObject.get(row.Objct_Id) ?? []).join(', '),
        renderCell: (params) => (
          <Stack direction="row" spacing={0.5} useFlexGap sx={{ flexWrap: 'wrap', alignItems: 'center' }}>
            {(rolesByObject.get(params.row.Objct_Id) ?? []).map((r) => (
              <Chip key={r} size="small" variant="outlined" color="primary" label={r} />
            ))}
          </Stack>
        ),
      },
      {
        field: 'Objct_St',
        headerName: 'Status',
        width: 110,
        renderCell: (params) =>
          params.row.Objct_St === 'V' ? (
            <Chip size="small" color="success" label="Aktivan" />
          ) : (
            <Chip size="small" variant="outlined" label={params.row.Objct_St ?? '—'} />
          ),
      },
    ],
    [types.items, rolesByObject],
  );

  if (objects.error) return <Alert severity="error">{objects.error}</Alert>;
  if (objects.items === null) return <LoadingBlock rows={4} />;
  return (
    <AppDataGrid
      rows={objects.items}
      columns={columns}
      getRowId={(o) => o.Objct_Id}
      getRowHeight={() => 'auto'}
      sx={{ '& .MuiDataGrid-cell': { py: 0.75 } }}
    />
  );
}

/**
 * "Prava pristupa": users (editable, with their roles), the role x screen permission matrix (editable), and read-only
 * views of the roles and the securable objects (screens). The selected sub-tab is kept in the URL (?sub=prava).
 */
export default function AccessRightsTab() {
  const [tab, setTab] = useUrlTab(TAB_IDS, 'sub');

  const content = [<UsersTab key="u" />, <PermissionMatrixTab key="m" />, <RolesView key="r" />, <ObjectsView key="o" />][tab];

  return (
    <>
      <Tabs
        value={tab}
        onChange={(_, value: number) => setTab(value)}
        variant="scrollable"
        sx={{ borderBottom: 1, borderColor: 'divider' }}
        aria-label="Podjela prava pristupa"
      >
        <Tab label="Korisnici" />
        <Tab label="Prava po ulogama" />
        <Tab label="Uloge" />
        <Tab label="Objekti (ekrani)" />
      </Tabs>
      <Box sx={{ pt: 2 }}>{content}</Box>
    </>
  );
}
