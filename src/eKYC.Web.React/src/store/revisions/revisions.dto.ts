import { CL_Doc_Revisions } from '../../models';

export type RevisionsState = {
  revisions: CL_Doc_Revisions[] | null;
  pending: boolean;
  pendingAction: boolean;
  error: string | null;
};
