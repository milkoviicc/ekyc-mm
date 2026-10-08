import AdminPanelSettingsOutlinedIcon from '@mui/icons-material/AdminPanelSettingsOutlined';
import AssessmentOutlinedIcon from '@mui/icons-material/AssessmentOutlined';
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
import FactCheckOutlinedIcon from '@mui/icons-material/FactCheckOutlined';
import ListAltOutlinedIcon from '@mui/icons-material/ListAltOutlined';
import ManageSearchOutlinedIcon from '@mui/icons-material/ManageSearchOutlined';
import PeopleOutlineOutlinedIcon from '@mui/icons-material/PeopleOutlineOutlined';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import { ReactElement } from 'react';
import { Route } from 'react-router-dom';
import RequireAccess from '../modules/shared/RequireAccess';
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

export type NavGroup = 'Rad' | 'Pregled' | 'Sustav';

export type AppRoute = {
  path: string;
  label: string;
  group: NavGroup;
  /** A_Objct.Asmbly_Cd codes (tabs); the user needs at least one of them to open this screen. Empty = open to everyone. */
  objectCodes: readonly string[];
  icon: ReactElement;
  element: ReactElement;
};

/** Same screens, labels and URLs as the Blazor app, grouped by what the user is doing. */
export const APP_ROUTES: AppRoute[] = [
  {
    path: ROUTES.dashboard,
    label: 'Nadzorna ploča',
    group: 'Rad',
    objectCodes: ['tabNadzor'],
    icon: <DashboardOutlinedIcon />,
    element: <Dashboard />,
  },
  {
    path: ROUTES.clientAnalysis,
    label: 'Klijenti - analiza',
    group: 'Rad',
    objectCodes: ['tabClients'],
    icon: <ManageSearchOutlinedIcon />,
    element: <ClientAnalysis />,
  },
  {
    path: ROUTES.revisions,
    label: 'Revizija',
    group: 'Rad',
    objectCodes: ['tabRevision'],
    icon: <FactCheckOutlinedIcon />,
    element: <Revision />,
  },
  {
    path: ROUTES.clientOverview,
    label: 'Klijenti - pregled',
    group: 'Pregled',
    objectCodes: ['tabOverview'],
    icon: <VisibilityOutlinedIcon />,
    element: <ClientOverview />,
  },
  {
    path: ROUTES.reports,
    label: 'Izvješća',
    group: 'Pregled',
    objectCodes: ['tabReports'],
    icon: <AssessmentOutlinedIcon />,
    element: <Reports />,
  },
  {
    path: ROUTES.administration,
    label: 'Administriranje',
    group: 'Sustav',
    objectCodes: ['tabAdmin'],
    icon: <AdminPanelSettingsOutlinedIcon />,
    element: <Administration />,
  },
  {
    path: ROUTES.clients,
    label: 'Clients',
    group: 'Sustav',
    objectCodes: ['tabClients', 'tabAdmin'],
    icon: <PeopleOutlineOutlinedIcon />,
    element: <Clients />,
  },
  {
    path: ROUTES.referenceData,
    label: 'Reference Data',
    group: 'Sustav',
    objectCodes: ['tabClients', 'tabOverview', 'tabAdmin'],
    icon: <ListAltOutlinedIcon />,
    element: <ReferenceData />,
  },
];

export const NAV_GROUPS: NavGroup[] = ['Rad', 'Pregled', 'Sustav'];

export const renderAllRoutes = () =>
  APP_ROUTES.map((route) => (
    <Route key={route.path} path={route.path} element={<RequireAccess codes={route.objectCodes}>{route.element}</RequireAccess>} />
  ));
