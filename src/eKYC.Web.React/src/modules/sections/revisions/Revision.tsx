import { Alert, Box, Button, Paper } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import dayjs from 'dayjs';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useRevisionTypes } from '../../../hooks/useReferenceData';
import { isConflict } from '../../../http-common';
import { CL_Doc_Revisions, DocRevisionFilter } from '../../../models';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showSuccessMessage, showWarningMessage } from '../../../store/message';
import { createRevision, deleteRevision, getRevisions, updateRevision } from '../../../store/revisions';
import { formatDate, toApiDate } from '../../../utils/formatting';
import AppDataGrid from '../../shared/AppDataGrid';
import ConfirmDialog from '../../shared/ConfirmDialog';
import FilterGrid from '../../shared/FilterGrid';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import { SelectField } from '../../shared/SelectField';
import RevisionEditDialog from './RevisionEditDialog';

const REVISION_START_YEAR = 2016;

/** "Revizija" tab - list, create, edit and delete document revisions. */
export default function Revision() {
  const dispatch = useAppDispatch();
  const { revisions, pendingAction, error } = useAppSelector((state) => state.revisions);
  const revisionTypes = useRevisionTypes();

  const [filter, setFilter] = useState<DocRevisionFilter>({});
  const [selected, setSelected] = useState<CL_Doc_Revisions | null>(null);
  const [editing, setEditing] = useState<{ revision: CL_Doc_Revisions; isNew: boolean } | null>(null);
  const [confirmDelete, setConfirmDelete] = useState(false);

  const years = useMemo(() => {
    const last = dayjs().year() + 1;
    return Array.from({ length: last - REVISION_START_YEAR + 1 }, (_, i) => last - i);
  }, []);

  const load = useCallback(
    (f: DocRevisionFilter) => {
      setSelected(null);
      return dispatch(getRevisions(f));
    },
    [dispatch],
  );

  useEffect(() => {
    load({});
  }, [load]);

  const changeFilter = (next: DocRevisionFilter) => {
    setFilter(next);
    load(next);
  };

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
  const changeRevision = () => selected && setEditing({ isNew: false, revision: { ...selected } });

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
    load(filter);
  };

  const remove = async () => {
    if (!selected) return;
    setConfirmDelete(false);
    try {
      await dispatch(deleteRevision(selected.CL_Doc_Revision_Id)).unwrap();
      dispatch(showSuccessMessage('Revizija je obrisana.'));
      load(filter);
    } catch {
      // surfaced through the store's error state / network layer
    }
  };

  const columns = useMemo<GridColDef<CL_Doc_Revisions>[]>(
    () => [
      { field: 'Rev_Date', headerName: 'Ndnk. revizije', width: 140, valueFormatter: (v: string) => formatDate(v) },
      { field: 'Rev_Range_From', headerName: 'Od', width: 120, valueFormatter: (v: string | null) => formatDate(v) },
      { field: 'Rev_Range_To', headerName: 'Do', width: 120, valueFormatter: (v: string | null) => formatDate(v) },
      { field: 'Rev_Done_By', headerName: 'Revizor', flex: 1, minWidth: 160 },
      { field: 'Subject', headerName: 'Predmet', flex: 1.5, minWidth: 200 },
      { field: 'Recommendation', headerName: 'Preporuka', flex: 1.5, minWidth: 200, sortable: false },
    ],
    [],
  );

  return (
    <>
      <PageHeader title="Revizija" />

      <Paper sx={{ p: 2, mb: 2 }} elevation={1}>
        <FilterGrid>
          <SelectField
            label="Godina"
            value={filter.Year ?? null}
            onChange={(v) => changeFilter({ ...filter, Year: v })}
            options={years.map((y) => ({ value: y, label: String(y) }))}
          />
          <SelectField
            label="Vrsta revizije"
            value={filter.RevTypeId ?? null}
            onChange={(v) => changeFilter({ ...filter, RevTypeId: v })}
            options={(revisionTypes ?? []).map((rt) => ({ value: rt.RevTypeId, label: rt.RevTypeName }))}
          />
        </FilterGrid>
      </Paper>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      {revisions === null ? (
        error ? null : (
          <LoadingBlock />
        )
      ) : (
        <AppDataGrid
          rows={revisions}
          columns={columns}
          getRowId={(row) => row.CL_Doc_Revision_Id}
          onRowClick={(params) => setSelected(params.row)}
          getRowClassName={(params) => (params.row.CL_Doc_Revision_Id === selected?.CL_Doc_Revision_Id ? 'selected-row' : '')}
          sx={{ '& .selected-row': { backgroundColor: 'action.selected' }, '& .MuiDataGrid-row': { cursor: 'pointer' } }}
        />
      )}

      <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
        <Button variant="contained" onClick={newRevision}>
          Nova revizija
        </Button>
        <Button variant="outlined" disabled={!selected} onClick={changeRevision}>
          Promjena revizije
        </Button>
        <Button variant="outlined" color="error" disabled={!selected} onClick={() => setConfirmDelete(true)}>
          Brisanje revizije
        </Button>
      </Box>

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
        open={confirmDelete}
        title="Brisanje revizije"
        message={`Obrisati reviziju "${selected?.Subject ?? ''}"?`}
        confirmText="Obriši"
        onConfirm={remove}
        onCancel={() => setConfirmDelete(false)}
      />
    </>
  );
}
