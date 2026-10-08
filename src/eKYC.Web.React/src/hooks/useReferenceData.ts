import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import {
  getClientTypes,
  getProcessingStatuses,
  getRevisionTypes,
  getRiskEstimates,
} from '../store/referenceData';

/**
 * Lookup tables are loaded once (the first screen that needs them triggers the request) and then read from the store.
 * Each hook returns the list, or null while it is still loading.
 */
export function useClientTypes() {
  const dispatch = useAppDispatch();
  const items = useAppSelector((state) => state.referenceData.clientTypes.items);
  useEffect(() => {
    if (items === null) dispatch(getClientTypes());
  }, [items, dispatch]);
  return items;
}

export function useProcessingStatuses() {
  const dispatch = useAppDispatch();
  const items = useAppSelector((state) => state.referenceData.processingStatuses.items);
  useEffect(() => {
    if (items === null) dispatch(getProcessingStatuses());
  }, [items, dispatch]);
  return items;
}

export function useRiskEstimates() {
  const dispatch = useAppDispatch();
  const items = useAppSelector((state) => state.referenceData.riskEstimates.items);
  useEffect(() => {
    if (items === null) dispatch(getRiskEstimates());
  }, [items, dispatch]);
  return items;
}

export function useRevisionTypes() {
  const dispatch = useAppDispatch();
  const items = useAppSelector((state) => state.referenceData.revisionTypes.items);
  useEffect(() => {
    if (items === null) dispatch(getRevisionTypes());
  }, [items, dispatch]);
  return items;
}
