import HistoryIcon from '@mui/icons-material/History';
import { Alert, Chip } from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { GridColDef } from '@mui/x-data-grid';
import dayjs from 'dayjs';
import { useEffect, useMemo, useState } from 'react';
import { Operation_Log, OperationLogFilter } from '../../../models';
import { getUsers } from '../../../store/admin';
import { getAudit, getAuditObjectTypes } from '../../../store/audit';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { formatDate, formatDateTime, fullName, toApiDate } from '../../../utils/formatting';
import AppDataGrid from '../../shared/AppDataGrid';
import EmptyState from '../../shared/EmptyState';
import FilterBar, { ActiveFilter } from '../../shared/FilterBar';
import LoadingBlock from '../../shared/LoadingBlock';
import { SelectField } from '../../shared/SelectField';

const OPERATION_COLORS: Record<string, 'success' | 'warning' | 'error' | 'default'> = {
  INSERT: 'success',
  UPDATE: 'warning',
  DELETE: 'error',
};

/** "Dnevnik promjena" - read-only audit trail (Operation_Log). The newest 500 entries match the filter; the grid can export them to CSV. */
export default function AuditTab() {
  const dispatch = useAppDispatch();
  const { rows, objectTypes, pending, error } = useAppSelector((state) => state.audit);
  const users = useAppSelector((state) => state.admin.users);

  const [draft, setDraft] = useState<OperationLogFilter>({});
  const [applied, setApplied] = useState<OperationLogFilter>({});

  useEffect(() => {
    dispatch(getAudit({}));
    dispatch(getAuditObjectTypes());
    dispatch(getUsers());
  }, [dispatch]);

  const apply = (filter: OperationLogFilter) => {
    setDraft(filter);
    setApplied(filter);
    dispatch(getAudit(filter));
  };

  const active: ActiveFilter[] = [];
  if (applied.From) active.push({ key: 'from', label: `Od: ${formatDate(applied.From)}`, onDelete: () => apply({ ...applied, From: null }) });
  if (applied.To) active.push({ key: 'to', label: `Do: ${formatDate(applied.To)}`, onDelete: () => apply({ ...applied, To: null }) });
  if (applied.User_Id) {
    const name = users.items?.find((u) => u.Usr_Id === applied.User_Id)?.Lgn_Nm ?? applied.User_Id;
    active.push({ key: 'user', label: `Korisnik: ${name}`, onDelete: () => apply({ ...applied, User_Id: null }) });
  }
  if (applied.Object_Type) active.push({ key: 'type', label: `Vrsta: ${applied.Object_Type}`, onDelete: () => apply({ ...applied, Object_Type: null }) });

  const columns = useMemo<GridColDef<Operation_Log>[]>(
    () => [
      { field: 'Log_Timestamp', headerName: 'Vrijeme', width: 160, valueFormatter: (v: string) => formatDateTime(v) },
      {
        field: 'Lgn_Nm',
        headerName: 'Korisnik',
        width: 190,
        valueGetter: (_v, row) => (row.Lgn_Nm ? `${fullName(row.Usr_Nm_Fst, row.Usr_Nm_Lst)} (${row.Lgn_Nm})` : `#${row.User_Id}`),
      },
      {
        field: 'Operation_Type',
        headerName: 'Radnja',
        width: 110,
        renderCell: (params) => (
          <Chip size="small" variant="outlined" color={OPERATION_COLORS[params.row.Operation_Type] ?? 'default'} label={params.row.Operation_Type} />
        ),
      },
      { field: 'Object_Type', headerName: 'Vrsta', width: 160 },
      { field: 'Object_Code', headerName: 'Oznaka', width: 190 },
      { field: 'Log_Remark', headerName: 'Opis', flex: 1, minWidth: 280 },
    ],
    [],
  );

  const datePicker = (label: string, key: 'From' | 'To') => (
    <DatePicker
      label={label}
      value={draft[key] ? dayjs(draft[key]) : null}
      onChange={(value) => setDraft({ ...draft, [key]: toApiDate(value) })}
      slotProps={{ textField: { size: 'small', fullWidth: true }, field: { clearable: true } }}
    />
  );

  return (
    <>
      <FilterBar active={active} onApply={() => apply(draft)} onClear={() => apply({})}>
        {datePicker('Od', 'From')}
        {datePicker('Do', 'To')}
        <SelectField
          label="Korisnik"
          value={draft.User_Id ?? null}
          onChange={(v) => setDraft({ ...draft, User_Id: v })}
          options={(users.items ?? []).map((u) => ({ value: u.Usr_Id, label: `${u.Lgn_Nm ?? ''} (${fullName(u.Usr_Nm_Fst, u.Usr_Nm_Lst) || '—'})` }))}
        />
        <SelectField
          label="Vrsta zapisa"
          value={draft.Object_Type ?? null}
          onChange={(v) => setDraft({ ...draft, Object_Type: v })}
          options={(objectTypes ?? []).map((t) => ({ value: t, label: t }))}
        />
      </FilterBar>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {pending || rows === null ? (
        error ? null : <LoadingBlock />
      ) : rows.length === 0 ? (
        <EmptyState icon={<HistoryIcon />} title="Nema zapisa" description="Nijedna promjena ne odgovara odabranim filtrima." />
      ) : (
        <AppDataGrid rows={rows} columns={columns} getRowId={(r) => r.Log_Id} />
      )}
    </>
  );
}
