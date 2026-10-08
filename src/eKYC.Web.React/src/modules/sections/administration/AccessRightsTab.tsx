import { Alert, Box, Tab, Tabs } from '@mui/material';
import { GridColDef, GridValidRowModel } from '@mui/x-data-grid';
import { ReactElement, useEffect, useState } from 'react';
import { A_Apl_Prmtr, A_ISRole, A_ISRole_Objct, A_Objct, A_Objct_Typs, A_Usr, A_Usr_ISRole } from '../../../models';
import {
  getObjects,
  getObjectTypes,
  getParameters,
  getRoleObjects,
  getRoles,
  getUserRoles,
  getUsers,
} from '../../../store/admin';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { formatDate, formatDateTime } from '../../../utils/formatting';
import AppDataGrid from '../../shared/AppDataGrid';
import LoadingBlock from '../../shared/LoadingBlock';

type ListViewProps<R extends GridValidRowModel> = {
  items: R[] | null;
  error: string | null;
  columns: GridColDef<R>[];
  getRowId: (row: R) => number | string;
};

function ListView<R extends GridValidRowModel>({ items, error, columns, getRowId }: ListViewProps<R>) {
  if (error) return <Alert severity="error">{error}</Alert>;
  if (items === null) return <LoadingBlock />;
  return <AppDataGrid<R> rows={items} columns={columns} getRowId={getRowId} />;
}

/**
 * "Prava pristupa" - read-only views of the A_ security tables (users, roles, assignments, objects, permissions).
 * Create/edit is not built yet; these are the GET endpoints under api/admin/*.
 */
export default function AccessRightsTab() {
  const dispatch = useAppDispatch();
  const admin = useAppSelector((state) => state.admin);
  const [tab, setTab] = useState(0);

  useEffect(() => {
    dispatch(getUsers());
    dispatch(getRoles());
    dispatch(getUserRoles());
    dispatch(getRoleObjects());
    dispatch(getObjects());
    dispatch(getObjectTypes());
    dispatch(getParameters());
  }, [dispatch]);

  const tabs: { label: string; content: ReactElement }[] = [
    {
      label: 'Korisnici',
      content: (
        <ListView<A_Usr>
          items={admin.users.items}
          error={admin.users.error}
          getRowId={(r) => r.Usr_Id}
          columns={[
            { field: 'Usr_Id', headerName: 'Id', type: 'number', width: 80 },
            { field: 'Lgn_Nm', headerName: 'Korisničko ime', width: 150 },
            { field: 'Usr_Nm_Fst', headerName: 'Ime', width: 130 },
            { field: 'Usr_Nm_Lst', headerName: 'Prezime', width: 130 },
            { field: 'Email', headerName: 'Email', flex: 1, minWidth: 180 },
            { field: 'Usr_St', headerName: 'Status', width: 90 },
            { field: 'ISRol_Id', headerName: 'Uloga (ISRol_Id)', type: 'number', width: 140 },
            { field: 'Lst_Lgn_Dt', headerName: 'Zadnja prijava', width: 160, valueFormatter: (v: string | null) => formatDateTime(v) },
            { field: 'Logged', headerName: 'Prijavljen', type: 'boolean', width: 100 },
          ]}
        />
      ),
    },
    {
      label: 'Uloge',
      content: (
        <ListView<A_ISRole>
          items={admin.roles.items}
          error={admin.roles.error}
          getRowId={(r) => r.ISRol_Id}
          columns={[
            { field: 'ISRol_Id', headerName: 'Id', type: 'number', width: 80 },
            { field: 'ISRol_Nm', headerName: 'Naziv', width: 200 },
            { field: 'ISRol_St', headerName: 'Status', width: 100 },
            { field: 'Apl_Cd', headerName: 'Šifra aplikacije', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Korisnik - uloge',
      content: (
        <ListView<A_Usr_ISRole>
          items={admin.userRoles.items}
          error={admin.userRoles.error}
          getRowId={(r) => r.Usr_ISRol_Id}
          columns={[
            { field: 'Usr_ISRol_Id', headerName: 'Id', type: 'number', width: 80 },
            { field: 'Usr_Id', headerName: 'Korisnik', type: 'number', width: 100 },
            { field: 'ISRol_Id', headerName: 'Uloga', type: 'number', width: 100 },
            { field: 'Vld_From_Dt', headerName: 'Vrijedi od', width: 140, valueFormatter: (v: string | null) => formatDate(v) },
            { field: 'Vld_To_Dt', headerName: 'Vrijedi do', width: 140, valueFormatter: (v: string | null) => formatDate(v) },
          ]}
        />
      ),
    },
    {
      label: 'Prava uloga',
      content: (
        <ListView<A_ISRole_Objct>
          items={admin.roleObjects.items}
          error={admin.roleObjects.error}
          getRowId={(r) => r.ISRol_Objct_Id}
          columns={[
            { field: 'ISRol_Objct_Id', headerName: 'Id', type: 'number', width: 80 },
            { field: 'ISRol_Id', headerName: 'Uloga', type: 'number', width: 90 },
            { field: 'Objct_Id', headerName: 'Objekt', type: 'number', width: 90 },
            { field: 'Objct_Ttl', headerName: 'Naslov', flex: 1, minWidth: 180 },
            { field: 'Prnt_Objct_id', headerName: 'Roditelj', type: 'number', width: 100 },
            { field: 'Sqnc_No', headerName: 'Red', type: 'number', width: 80 },
            { field: 'Mdfr_Cd', headerName: 'Modifikator', width: 110 },
            { field: 'Hrchy_Lvl', headerName: 'Razina', type: 'number', width: 90 },
            { field: 'Hrchy_Path', headerName: 'Putanja', width: 160 },
          ]}
        />
      ),
    },
    {
      label: 'Objekti',
      content: (
        <ListView<A_Objct>
          items={admin.objects.items}
          error={admin.objects.error}
          getRowId={(r) => r.Objct_Id}
          columns={[
            { field: 'Objct_Id', headerName: 'Id', type: 'number', width: 80 },
            { field: 'Objct_Typ_Id', headerName: 'Tip', type: 'number', width: 80 },
            { field: 'Objct_Nm', headerName: 'Naziv', width: 200 },
            { field: 'Objct_Dspn', headerName: 'Opis', flex: 1, minWidth: 180 },
            { field: 'Objct_Call', headerName: 'Poziv', width: 180 },
            { field: 'Asmbly_Cd', headerName: 'Šifra', width: 140 },
            { field: 'Objct_St', headerName: 'Status', width: 90 },
          ]}
        />
      ),
    },
    {
      label: 'Tipovi objekata',
      content: (
        <ListView<A_Objct_Typs>
          items={admin.objectTypes.items}
          error={admin.objectTypes.error}
          getRowId={(r) => r.Objct_Typ_Id}
          columns={[
            { field: 'Objct_Typ_Id', headerName: 'Id', type: 'number', width: 80 },
            { field: 'Typ_Nm', headerName: 'Naziv', width: 200 },
            { field: 'Typ_Cd', headerName: 'Šifra', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Parametri',
      content: (
        <ListView<A_Apl_Prmtr>
          items={admin.parameters.items}
          error={admin.parameters.error}
          getRowId={(r) => r.Apl_Id}
          columns={[
            { field: 'Apl_Id', headerName: 'Id', type: 'number', width: 80 },
            { field: 'Prmtr_Cd', headerName: 'Šifra', width: 240 },
            { field: 'Prmtr_Val', headerName: 'Vrijednost', flex: 1, minWidth: 220 },
            { field: 'Prmtr_Dspn', headerName: 'Opis', flex: 1, minWidth: 220 },
          ]}
        />
      ),
    },
  ];

  return (
    <>
      <Tabs value={tab} onChange={(_, value: number) => setTab(value)} variant="scrollable" sx={{ borderBottom: 1, borderColor: 'divider' }}>
        {tabs.map((t) => (
          <Tab key={t.label} label={t.label} />
        ))}
      </Tabs>
      <Box sx={{ pt: 2 }}>{tabs[tab].content}</Box>
    </>
  );
}
