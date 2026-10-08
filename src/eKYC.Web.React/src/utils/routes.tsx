import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';
import AssessmentIcon from '@mui/icons-material/Assessment';
import DashboardIcon from '@mui/icons-material/Dashboard';
import FactCheckIcon from '@mui/icons-material/FactCheck';
import ListIcon from '@mui/icons-material/List';
import PeopleIcon from '@mui/icons-material/People';
import SearchIcon from '@mui/icons-material/Search';
import VisibilityIcon from '@mui/icons-material/Visibility';
import { ReactElement } from 'react';
import { Route } from 'react-router-dom';
import Administration from '../modules/sections/administration/Administration';
import ClientAnalysis from '../modules/sections/clientAnalysis/ClientAnalysis';
import ClientOverview from '../modules/sections/clientAnalysis/ClientOverview';
import Clients from '../modules/sections/clients/Clients';
import Dashboard from '../modules/sections/dashboard/Dashboard';
import ReferenceData from '../modules/sections/referenceData/ReferenceData';
import Reports from '../modules/sections/reports/Reports';
import Revision from '../modules/sections/revisions/Revision';

export const ROUTES = {
  dashboard: '/',
  clientAnalysis: '/klijenti-analiza',
  clientOverview: '/klijenti-pregled',
  reports: '/izvjesca',
  revisions: '/revizija',
  administration: '/administriranje',
  clients: '/clients',
  referenceData: '/reference-data',
} as const;

export type AppRoute = {
  path: string;
  label: string;
  icon: ReactElement;
  element: ReactElement;
};

/** Same screens, labels and URLs as the Blazor app's NavMenu. */
export const APP_ROUTES: AppRoute[] = [
  { path: ROUTES.dashboard, label: 'Nadzorna ploča', icon: <DashboardIcon />, element: <Dashboard /> },
  { path: ROUTES.clientAnalysis, label: 'Klijenti - analiza', icon: <SearchIcon />, element: <ClientAnalysis /> },
  { path: ROUTES.clientOverview, label: 'Klijenti - pregled', icon: <VisibilityIcon />, element: <ClientOverview /> },
  { path: ROUTES.reports, label: 'Izvješća', icon: <AssessmentIcon />, element: <Reports /> },
  { path: ROUTES.revisions, label: 'Revizija', icon: <FactCheckIcon />, element: <Revision /> },
  { path: ROUTES.administration, label: 'Administriranje', icon: <AdminPanelSettingsIcon />, element: <Administration /> },
  { path: ROUTES.clients, label: 'Clients', icon: <PeopleIcon />, element: <Clients /> },
  { path: ROUTES.referenceData, label: 'Reference Data', icon: <ListIcon />, element: <ReferenceData /> },
];

export const renderAllRoutes = () =>
  APP_ROUTES.map((route) => <Route key={route.path} path={route.path} element={route.element} />);
