import { Alert, Box, Paper, Tab, Tabs } from '@mui/material';
import { GridColDef, GridValidRowModel } from '@mui/x-data-grid';
import { ReactElement, useEffect, useState } from 'react';
import {
  ClientProcessingStatus,
  ClientProcessingStatusTransition,
  ClientType,
  ClState,
  DocumentType,
  OwnershipType,
  RevisionType,
  RiskClass,
  RiskEstimate,
} from '../../../models';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import {
  getClientTypes,
  getDocumentTypes,
  getOwnershipTypes,
  getProcessingStatuses,
  getProcessingStatusTransitions,
  getRevisionTypes,
  getRiskClasses,
  getRiskEstimates,
  getStates,
} from '../../../store/referenceData';
import AppDataGrid from '../../shared/AppDataGrid';
import LoadingBlock from '../../shared/LoadingBlock';
import PageHeader from '../../shared/PageHeader';

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

/** Read-only view of every lookup table behind api/reference-data/*. */
export default function ReferenceData() {
  const dispatch = useAppDispatch();
  const ref = useAppSelector((state) => state.referenceData);
  const [tab, setTab] = useState(0);

  useEffect(() => {
    dispatch(getStates());
    dispatch(getClientTypes());
    dispatch(getRiskClasses());
    dispatch(getRiskEstimates());
    dispatch(getOwnershipTypes());
    dispatch(getDocumentTypes());
    dispatch(getProcessingStatuses());
    dispatch(getProcessingStatusTransitions());
    dispatch(getRevisionTypes());
  }, [dispatch]);

  const tabs: { label: string; content: ReactElement }[] = [
    {
      label: 'States',
      content: (
        <ListView<ClState>
          items={ref.states.items}
          error={ref.states.error}
          getRowId={(r) => r.ClStateId}
          columns={[
            { field: 'StateCd', headerName: 'Code', width: 120 },
            { field: 'StateNm', headerName: 'Name', flex: 1 },
            { field: 'PhoneNum', headerName: 'Phone', width: 140 },
          ]}
        />
      ),
    },
    {
      label: 'Client Types',
      content: (
        <ListView<ClientType>
          items={ref.clientTypes.items}
          error={ref.clientTypes.error}
          getRowId={(r) => r.ClntTypId}
          columns={[
            { field: 'ClntTypCd', headerName: 'Code', width: 120 },
            { field: 'ClntTypDspn', headerName: 'Description', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Risk Classes',
      content: (
        <ListView<RiskClass>
          items={ref.riskClasses.items}
          error={ref.riskClasses.error}
          getRowId={(r) => r.RskClsId}
          columns={[
            { field: 'RskClsId', headerName: 'Id', type: 'number', width: 100 },
            { field: 'RskClsDspn', headerName: 'Description', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Risk Estimates',
      content: (
        <ListView<RiskEstimate>
          items={ref.riskEstimates.items}
          error={ref.riskEstimates.error}
          getRowId={(r) => r.RskEstId}
          columns={[
            { field: 'LowerPoints', headerName: 'Lower', type: 'number', width: 100 },
            { field: 'UpperPoints', headerName: 'Upper', type: 'number', width: 100 },
            { field: 'RiskLevel', headerName: 'Risk Level', width: 180 },
            { field: 'AnalysisType', headerName: 'Analysis Type', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Ownership Types',
      content: (
        <ListView<OwnershipType>
          items={ref.ownershipTypes.items}
          error={ref.ownershipTypes.error}
          getRowId={(r) => r.OwnrshpTypId}
          columns={[
            { field: 'OwnrshpTypCd', headerName: 'Code', width: 120 },
            { field: 'OwnrshpTypDspn', headerName: 'Description', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Document Types',
      content: (
        <ListView<DocumentType>
          items={ref.documentTypes.items}
          error={ref.documentTypes.error}
          getRowId={(r) => r.ClDocTypId}
          columns={[
            { field: 'DocTypeCd', headerName: 'Code', width: 140 },
            { field: 'DocTypeNm', headerName: 'Name', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Processing Statuses',
      content: (
        <ListView<ClientProcessingStatus>
          items={ref.processingStatuses.items}
          error={ref.processingStatuses.error}
          getRowId={(r) => r.ClntPrcsStId}
          columns={[
            { field: 'ClntPrcsStCd', headerName: 'Code', width: 160 },
            { field: 'Status', headerName: 'Status', width: 200 },
            { field: 'ClntPrcsStDspn', headerName: 'Description', flex: 1 },
          ]}
        />
      ),
    },
    {
      label: 'Status Transitions',
      content: (
        <ListView<ClientProcessingStatusTransition>
          items={ref.processingStatusTransitions.items}
          error={ref.processingStatusTransitions.error}
          getRowId={(r) => r.ClClntPrcsStToId}
          columns={[
            { field: 'ClntPrcsStCdFr', headerName: 'From', width: 200 },
            { field: 'ClntPrcsStCdTo', headerName: 'To', width: 200 },
          ]}
        />
      ),
    },
    {
      label: 'Revision Types',
      content: (
        <ListView<RevisionType>
          items={ref.revisionTypes.items}
          error={ref.revisionTypes.error}
          getRowId={(r) => r.RevTypeId}
          columns={[
            { field: 'RevTypeCd', headerName: 'Code', width: 120 },
            { field: 'RevTypeName', headerName: 'Name', flex: 1 },
          ]}
        />
      ),
    },
  ];

  return (
    <>
      <PageHeader title="Reference Data" />
      <Paper elevation={1}>
        <Tabs value={tab} onChange={(_, value: number) => setTab(value)} variant="scrollable">
          {tabs.map((t) => (
            <Tab key={t.label} label={t.label} />
          ))}
        </Tabs>
        <Box sx={{ p: 2 }}>{tabs[tab].content}</Box>
      </Paper>
    </>
  );
}
