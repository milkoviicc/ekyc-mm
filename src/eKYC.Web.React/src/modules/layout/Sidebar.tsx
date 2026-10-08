import { Box, List, ListItemButton, ListItemIcon, ListItemText, Tooltip, Typography } from '@mui/material';
import { alpha } from '@mui/material/styles';
import { NavLink, useLocation } from 'react-router-dom';
import { usePermissions } from '../../hooks/usePermissions';
import { APP_ROUTES, NAV_GROUPS } from '../../utils/routes';

type Props = {
  /** Icons-only rail (desktop). */
  collapsed?: boolean;
  /** Called after navigating, so the mobile drawer can close itself. */
  onNavigate?: () => void;
};

/** Navigation grouped by task (Rad / Pregled / Sustav). Collapsed it becomes an icon rail with tooltips. */
export default function Sidebar({ collapsed = false, onNavigate }: Props) {
  const { pathname } = useLocation();
  const { canOpen } = usePermissions();
  const visible = APP_ROUTES.filter((r) => canOpen(r.objectCodes));
  const isActive = (path: string) => (path === '/' ? pathname === '/' : pathname.startsWith(path));

  return (
    <Box component="nav" aria-label="Glavni izbornik" sx={{ px: collapsed ? 1 : 1.5, pb: 2, overflowY: 'auto', flex: 1 }}>
      {NAV_GROUPS.filter((group) => visible.some((r) => r.group === group)).map((group) => (
        <List
          key={group}
          dense
          disablePadding
          subheader={
            collapsed ? (
              <Box sx={{ height: 12 }} />
            ) : (
              <Typography
                variant="overline"
                color="text.secondary"
                sx={{ display: 'block', px: 1.5, pt: 2, pb: 0.5, letterSpacing: '0.08em', lineHeight: 1.6 }}
              >
                {group}
              </Typography>
            )
          }
        >
          {visible.filter((r) => r.group === group).map((route) => {
            const active = isActive(route.path);
            const button = (
              <ListItemButton
                key={route.path}
                component={NavLink}
                to={route.path}
                selected={active}
                onClick={onNavigate}
                aria-current={active ? 'page' : undefined}
                sx={(theme) => ({
                  my: 0.25,
                  minHeight: 42,
                  justifyContent: collapsed ? 'center' : 'flex-start',
                  px: collapsed ? 1 : 1.5,
                  color: active ? 'primary.main' : 'text.primary',
                  '&.Mui-selected': {
                    backgroundColor: alpha(theme.palette.primary.main, 0.1),
                    '&:hover': { backgroundColor: alpha(theme.palette.primary.main, 0.14) },
                  },
                })}
              >
                <ListItemIcon sx={{ minWidth: collapsed ? 0 : 36, color: 'inherit', justifyContent: 'center' }}>
                  {route.icon}
                </ListItemIcon>
                {!collapsed && (
                  <ListItemText primary={route.label} slotProps={{ primary: { noWrap: true, sx: { fontWeight: active ? 650 : 500 } } }} />
                )}
              </ListItemButton>
            );
            return collapsed ? (
              <Tooltip key={route.path} title={route.label} placement="right">
                {button}
              </Tooltip>
            ) : (
              button
            );
          })}
        </List>
      ))}
    </Box>
  );
}
