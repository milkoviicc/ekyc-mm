import { CL_Clnt } from '../../models';

export type ClientsState = {
  clients: CL_Clnt[] | null;
  pending: boolean;
  pendingAction: boolean;
  error: string | null;
};
