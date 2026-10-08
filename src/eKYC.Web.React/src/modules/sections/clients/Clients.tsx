import { Alert } from '@mui/material';
import { GridColDef } from '@mui/x-data-grid';
import { useEffect, useMemo, useState } from 'react';
import { useProcessingStatuses, useRiskEstimates } from '../../../hooks/useReferenceData';
import { isConflict } from '../../../http-common';
import { CL_Clnt } from '../../../models';
import { getClients, updateClient } from '../../../store/clients';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showSuccessMessage, showWarningMessage } from '../../../store/message';
import AppDataGrid from '../../shared/AppDataGrid';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';
import ClientEditDialog from './ClientEditDialog';

/** Dev/maintenance grid over CL_Clnt (api/clients). Click a row to edit it. */
export default function Clients() {
  const dispatch = useAppDispatch();
  const { clients, pendingAction, error } = useAppSelector((state) => state.clients);
  const processingStatuses = useProcessingStatuses();
  const riskEstimates = useRiskEstimates();

  const [editing, setEditing] = useState<CL_Clnt | null>(null);

  useEffect(() => {
    dispatch(getClients());
  }, [dispatch]);

  const save = async (client: CL_Clnt) => {
    try {
      await dispatch(updateClient(client)).unwrap();
      dispatch(showSuccessMessage('Klijent je ažuriran.'));
    } catch (e) {
      if (isConflict(e)) {
        dispatch(showWarningMessage('Klijent je u međuvremenu promijenjen od strane drugog korisnika. Podaci su osvježeni.'));
      } else {
        return; // keep the dialog open
      }
    }
    setEditing(null);
    dispatch(getClients());
  };

  const columns = useMemo<GridColDef<CL_Clnt>[]>(
    () => [
      { field: 'Clnt_Id', headerName: 'Id', type: 'number', width: 90 },
      { field: 'Clnt_Typ_Cd', headerName: 'Type', width: 90 },
      { field: 'Clnt_St', headerName: 'Status', width: 90 },
      { field: 'Clnt_Prcsng_St', headerName: 'Processing Status', width: 190 },
      { field: 'Rsk_Pnts', headerName: 'Risk Points', type: 'number', width: 120 },
      { field: 'PEP_Ind', headerName: 'PEP', width: 150 },
      { field: 'WtchLst_Ind', headerName: 'Watchlist', width: 110 },
    ],
    [],
  );

  return (
    <>
      <PageHeader title="Clients" subtitle="Click a row to edit." />
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      {clients === null ? (
        error ? null : (
          <LoadingBlock />
        )
      ) : clients.length === 0 ? (
        <Alert severity="info">No clients found.</Alert>
      ) : (
        <AppDataGrid
          rows={clients}
          columns={columns}
          getRowId={(row) => row.Clnt_Id}
          onRowClick={(params) => setEditing({ ...params.row })}
          sx={{ '& .MuiDataGrid-row': { cursor: 'pointer' } }}
        />
      )}

      {editing && (
        <ClientEditDialog
          key={`${editing.Clnt_Id}-${editing.row_version}`}
          open
          client={editing}
          processingStatuses={processingStatuses}
          riskEstimates={riskEstimates}
          saving={pendingAction}
          onCancel={() => setEditing(null)}
          onSave={save}
        />
      )}
    </>
  );
}
