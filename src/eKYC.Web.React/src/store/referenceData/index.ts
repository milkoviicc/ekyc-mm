import { combineReducers } from '@reduxjs/toolkit';
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
} from '../../models';
import { createListFeature } from '../utils/createListFeature';

// One list feature per endpoint of ReferenceDataController (api/reference-data/*).
export const states = createListFeature<ClState>('referenceData/states', '/api/reference-data/states');
export const clientTypes = createListFeature<ClientType>('referenceData/clientTypes', '/api/reference-data/client-types');
export const riskClasses = createListFeature<RiskClass>('referenceData/riskClasses', '/api/reference-data/risk-classes');
export const riskEstimates = createListFeature<RiskEstimate>(
  'referenceData/riskEstimates',
  '/api/reference-data/risk-estimates',
);
export const ownershipTypes = createListFeature<OwnershipType>(
  'referenceData/ownershipTypes',
  '/api/reference-data/ownership-types',
);
export const documentTypes = createListFeature<DocumentType>(
  'referenceData/documentTypes',
  '/api/reference-data/document-types',
);
export const processingStatuses = createListFeature<ClientProcessingStatus>(
  'referenceData/processingStatuses',
  '/api/reference-data/processing-statuses',
);
export const processingStatusTransitions = createListFeature<ClientProcessingStatusTransition>(
  'referenceData/processingStatusTransitions',
  '/api/reference-data/processing-status-transitions',
);
export const revisionTypes = createListFeature<RevisionType>(
  'referenceData/revisionTypes',
  '/api/reference-data/revision-types',
);

export const getStates = states.fetchAll;
export const getClientTypes = clientTypes.fetchAll;
export const getRiskClasses = riskClasses.fetchAll;
export const getRiskEstimates = riskEstimates.fetchAll;
export const getOwnershipTypes = ownershipTypes.fetchAll;
export const getDocumentTypes = documentTypes.fetchAll;
export const getProcessingStatuses = processingStatuses.fetchAll;
export const getProcessingStatusTransitions = processingStatusTransitions.fetchAll;
export const getRevisionTypes = revisionTypes.fetchAll;

export const referenceDataReducer = combineReducers({
  states: states.reducer,
  clientTypes: clientTypes.reducer,
  riskClasses: riskClasses.reducer,
  riskEstimates: riskEstimates.reducer,
  ownershipTypes: ownershipTypes.reducer,
  documentTypes: documentTypes.reducer,
  processingStatuses: processingStatuses.reducer,
  processingStatusTransitions: processingStatusTransitions.reducer,
  revisionTypes: revisionTypes.reducer,
});
