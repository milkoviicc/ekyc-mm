import AddIcon from '@mui/icons-material/Add';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import FactCheckOutlinedIcon from '@mui/icons-material/FactCheckOutlined';
import { Alert, Button } from '@mui/material';
import { GridActionsCellItem, GridColDef } from '@mui/x-data-grid';
import dayjs from 'dayjs';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { usePermissions } from '../../../hooks/usePermissions';
import { useRevisionTypes } from '../../../hooks/useReferenceData';
import { isConflict } from '../../../http-common';
import { CL_Doc_Revisions, DocRevisionFilter } from '../../../models';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showSuccessMessage, showWarningMessage } from '../../../store/message';
import { createRevision, deleteRevision, getRevisions, updateRevision } from '../../../store/revisions';
import { formatDate, toApiDate } from '../../../utils/formatting';
import AppDataGrid, { clickableRowsSx } from '../../shared/AppDataGrid';
import ConfirmDialog from '../../shared/ConfirmDialog';
import EmptyState from '../../shared/EmptyState';
import FilterBar, { ActiveFilter } from '../../shared/FilterBar';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import { SelectField } from '../../shared/SelectField';
import RevisionEditDialog from './RevisionEditDialog';

const REVISION_START_YEAR = 2016;

/** "Revizija" tab - list, create, edit and delete document revisions. Click a row (or the pencil) to edit it. */
export default function Revision() {
  const dispatch = useAppDispatch();
  const { revisions, pendingAction, error } = useAppSelector((state) => state.revisions);
  const revisionTypes = useRevisionTypes();
  // Same roles as the API (CL_Doc_RevisionsController): everyone with the screen can read, only these can change. The server enforces it either way.
  const { hasRole } = usePermissions();
  const canWrite = hasRole('UNOS', 'ADMIN', 'REVIZIJA');

  const [draft, setDraft] = useState<DocRevisionFilter>({});
  const [applied, setApplied] = useState<DocRevisionFilter>({});
  const [editing, setEditing] = useState<{ revision: CL_Doc_Revisions; isNew: boolean } | null>(null);
  const [toDelete, setToDelete] = useState<CL_Doc_Revisions | null>(null);

  const years = useMemo(() => {
    const last = dayjs().year() + 1;
    return Array.from({ length: last - REVISION_START_YEAR + 1 }, (_, i) => last - i);
  }, []);

  const load = useCallback((f: DocRevisionFilter) => dispatch(getRevisions(f)), [dispatch]);

  useEffect(() => {
    load({});
  }, [load]);

  const apply = (filter: DocRevisionFilter) => {
    setDraft(filter);
    setApplied(filter);
    load(filter);
  };

  const active: ActiveFilter[] = [];
  if (applied.Year) active.push({ key: 'year', label: `Godina: ${applied.Year}`, onDelete: () => apply({ ...applied, Year: null }) });
  if (applied.RevTypeId) {
    const name = revisionTypes?.find((r) => r.RevTypeId === applied.RevTypeId)?.RevTypeName ?? applied.RevTypeId;
    active.push({ key: 'type', label: `Vrsta: ${name}`, onDelete: () => apply({ ...applied, RevTypeId: null }) });
  }

  const newRevision = () =>
    setEditing({
      isNew: true,
      revision: {
        CL_Doc_Revision_Id: 0,
        CL_Doc_Typ_Rev_Id: 0,
        Rev_Date: toApiDate(dayjs()) as string,
        Rev_Done_By: '',
        Subject: '',
        row_version: 0,
        tenant_id: 0,
      },
    });

  // A copy is edited, so cancelling the dialog does not leave the grid showing unsaved changes.
  const edit = (revision: CL_Doc_Revisions) => setEditing({ isNew: false, revision: { ...revision } });

  const save = async (revision: CL_Doc_Revisions) => {
    const isNew = editing?.isNew ?? false;
    try {
      if (isNew) {
        await dispatch(createRevision(revision)).unwrap();
        dispatch(showSuccessMessage('Revizija je spremljena.'));
      } else {
        await dispatch(updateRevision(revision)).unwrap();
        dispatch(showSuccessMessage('Revizija je ažurirana.'));
      }
    } catch (e) {
      if (isConflict(e)) {
        dispatch(showWarningMessage('Revizija je u međuvremenu promijenjena od strane drugog korisnika. Podaci su osvježeni.'));
      } else {
        return; // the request failed for another reason - keep the dialog open
      }
    }
    setEditing(null);
    load(applied);
  };

  const remove = async () => {
    if (!toDelete) return;
    const target = toDelete;
    setToDelete(null);
    try {
      await dispatch(deleteRevision(target.CL_Doc_Revision_Id)).unwrap();
      dispatch(showSuccessMessage('Revizija je obrisana.'));
      load(applied);
    } catch {
      // surfaced through the store's error state / network layer
    }
  };

  const columns = useMemo<GridColDef<CL_Doc_Revisions>[]>(
    () => [
      { field: 'Rev_Date', headerName: 'Nadnevak', width: 120, valueFormatter: (v: string) => formatDate(v) },
      {
        field: 'Rev_Range_From',
        headerName: 'Razdoblje',
        width: 200,
        valueGetter: (_v, row) => row.Rev_Range_From ?? '',
        renderCell: (params) =>
          params.row.Rev_Range_From || params.row.Rev_Range_To
            ? `${formatDate(params.row.Rev_Range_From) || '…'} – ${formatDate(params.row.Rev_Range_To) || '…'}`
            : '—',
      },
      { field: 'Rev_Done_By', headerName: 'Revizor', flex: 1, minWidth: 160 },
      { field: 'Subject', headerName: 'Predmet', flex: 1.5, minWidth: 220 },
      { field: 'Recommendation', headerName: 'Preporuka', flex: 1.5, minWidth: 220, sortable: false },
      ...(canWrite
        ? [
            {
        field: 'actions',
        type: 'actions',
        headerName: '',
        width: 96,
        getActions: ({ row }) => [
          <GridActionsCellItem key="edit" icon={<EditOutlinedIcon />} label="Promijeni reviziju" onClick={() => edit(row)} />,
          <GridActionsCellItem key="delete" icon={<DeleteOutlinedIcon />} label="Obriši reviziju" onClick={() => setToDelete(row)} />,
        ],
      },
          ] as GridColDef<CL_Doc_Revisions>[]
        : []),
    ],
    [canWrite],
  );

  return (
    <>
      <PageHeader
        title="Revizija"
        subtitle="Evidencija provedenih revizija i preporuka."
        actions={
          canWrite ? (
            <Button variant="contained" startIcon={<AddIcon />} onClick={newRevision}>
              Nova revizija
            </Button>
          ) : undefined
        }
      />

      <FilterBar active={active} onApply={() => apply(draft)} onClear={() => apply({})}>
        <SelectField
          label="Godina"
          value={draft.Year ?? null}
          onChange={(v) => setDraft({ ...draft, Year: v })}
          options={years.map((y) => ({ value: y, label: String(y) }))}
        />
        <SelectField
          label="Vrsta revizije"
          value={draft.RevTypeId ?? null}
          onChange={(v) => setDraft({ ...draft, RevTypeId: v })}
          options={(revisionTypes ?? []).map((rt) => ({ value: rt.RevTypeId, label: rt.RevTypeName }))}
        />
      </FilterBar>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} action={<Button color="inherit" size="small" onClick={() => load(applied)}>Pokušaj ponovno</Button>}>
          {error}
        </Alert>
      )}
      {revisions === null ? (
        error ? null : <LoadingBlock />
      ) : revisions.length === 0 ? (
        <EmptyState
          icon={<FactCheckOutlinedIcon />}
          title="Nema revizija"
          description={active.length ? 'Nijedna revizija ne odgovara filtrima.' : 'Dodajte prvu reviziju.'}
          action={
            active.length ? <Button onClick={() => apply({})}>Očisti filtre</Button> : canWrite ? <Button variant="contained" onClick={newRevision}>Nova revizija</Button> : undefined
          }
        />
      ) : (
        <AppDataGrid
          rows={revisions}
          columns={columns}
          getRowId={(row) => row.CL_Doc_Revision_Id}
          getRowHeight={() => 'auto'}
          onRowClick={(params, event) => {
            // the action buttons handle their own clicks
            if ((event.target as HTMLElement).closest('.MuiDataGrid-actionsCell') || !canWrite) return;
            edit(params.row);
          }}
          sx={[clickableRowsSx, { '& .MuiDataGrid-cell': { py: 1 } }]}
        />
      )}

      {editing && (
        <RevisionEditDialog
          key={`${editing.isNew}-${editing.revision.CL_Doc_Revision_Id}-${editing.revision.row_version}`}
          open
          revision={editing.revision}
          isNew={editing.isNew}
          revisionTypes={revisionTypes}
          saving={pendingAction}
          onCancel={() => setEditing(null)}
          onSave={save}
        />
      )}

      <ConfirmDialog
        open={Boolean(toDelete)}
        title="Brisanje revizije"
        message={`Obrisati reviziju „${toDelete?.Subject ?? ''}“? Ovu radnju nije moguće poništiti.`}
        confirmText="Obriši"
        onConfirm={remove}
        onCancel={() => setToDelete(null)}
      />
    </>
  );
}
