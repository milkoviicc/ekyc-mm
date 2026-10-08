import { List, ListItemButton, ListItemIcon, ListItemText, Toolbar } from '@mui/material';
import { NavLink, useLocation } from 'react-router-dom';
import { APP_ROUTES } from '../../utils/routes';

type Props = {
  /** Called after navigating, so the mobile drawer can close itself. */
  onNavigate?: () => void;
};

export default function Sidebar({ onNavigate }: Props) {
  const { pathname } = useLocation();

  return (
    <>
      <Toolbar />
      <List>
        {APP_ROUTES.map((route) => (
          <ListItemButton
            key={route.path}
            component={NavLink}
            to={route.path}
            selected={route.path === '/' ? pathname === '/' : pathname.startsWith(route.path)}
            onClick={onNavigate}
          >
            <ListItemIcon>{route.icon}</ListItemIcon>
            <ListItemText primary={route.label} />
          </ListItemButton>
        ))}
      </List>
    </>
  );
}
