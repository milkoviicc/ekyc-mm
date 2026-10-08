import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import DarkModeOutlinedIcon from '@mui/icons-material/DarkModeOutlined';
import LightModeOutlinedIcon from '@mui/icons-material/LightModeOutlined';
import MenuIcon from '@mui/icons-material/Menu';
import { Box, Drawer, IconButton, Link, Tooltip, Typography, useMediaQuery } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { ReactNode, useEffect, useState } from 'react';
import { Link as RouterLink, useLocation } from 'react-router-dom';
import { useColorMode } from '../../styles/ColorModeProvider';
import Sidebar from './Sidebar';
import UserMenu from './UserMenu';

const WIDTH_FULL = 252;
const WIDTH_RAIL = 72;
const COLLAPSE_KEY = 'ekyc.sidebarCollapsed';

const readCollapsed = () => {
  try {
    return localStorage.getItem(COLLAPSE_KEY) === '1';
  } catch {
    return false;
  }
};

function Brand({ collapsed }: { collapsed: boolean }) {
  return (
    <Link
      component={RouterLink}
      to="/"
      underline="none"
      color="text.primary"
      aria-label="eKYC - početna"
      sx={{ display: 'flex', alignItems: 'center', gap: 1.25, px: collapsed ? 0 : 2.5, justifyContent: collapsed ? 'center' : 'flex-start', height: 64 }}
    >
      <Box
        aria-hidden
        sx={{
          width: 32,
          height: 32,
          borderRadius: 2,
          display: 'grid',
          placeItems: 'center',
          color: 'primary.contrastText',
          background: (t) => `linear-gradient(135deg, ${t.palette.primary.main}, ${t.palette.info.main})`,
          fontWeight: 800,
          fontSize: 14,
          flex: 'none',
        }}
      >
        eK
      </Box>
      {!collapsed && (
        <Typography variant="h6" sx={{ letterSpacing: '-0.02em' }}>
          eKYC
        </Typography>
      )}
    </Link>
  );
}

/**
 * App shell: collapsible sidebar (icon rail), slim top bar with the theme toggle, content area.
 * On small screens the sidebar becomes a temporary drawer.
 */
export default function Layout({ children }: { children: ReactNode }) {
  const theme = useTheme();
  const desktop = useMediaQuery(theme.breakpoints.up('md'));
  const { mode, toggle } = useColorMode();
  const { pathname } = useLocation();

  const [collapsed, setCollapsed] = useState(readCollapsed);
  const [mobileOpen, setMobileOpen] = useState(false);

  const toggleCollapsed = () =>
    setCollapsed((c) => {
      try {
        localStorage.setItem(COLLAPSE_KEY, c ? '0' : '1');
      } catch {
        /* ignore */
      }
      return !c;
    });

  // Moving to another screen should start at the top, like a page load.
  useEffect(() => {
    window.scrollTo({ top: 0 });
  }, [pathname]);

  const railed = desktop && collapsed;
  const sidebarWidth = railed ? WIDTH_RAIL : WIDTH_FULL;

  const sidebar = (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <Brand collapsed={railed} />
      <Sidebar collapsed={railed} onNavigate={() => setMobileOpen(false)} />
      {desktop && (
        <Box sx={{ p: 1, borderTop: 1, borderColor: 'divider', display: 'flex', justifyContent: railed ? 'center' : 'flex-end' }}>
          <Tooltip title={railed ? 'Proširi izbornik' : 'Sažmi izbornik'} placement="right">
            <IconButton onClick={toggleCollapsed} aria-label={railed ? 'Proširi izbornik' : 'Sažmi izbornik'}>
              {railed ? <ChevronRightIcon /> : <ChevronLeftIcon />}
            </IconButton>
          </Tooltip>
        </Box>
      )}
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <Link
        href="#main"
        sx={{ position: 'absolute', left: -9999, '&:focus': { left: 16, top: 8, zIndex: 2000, bgcolor: 'background.paper', p: 1, borderRadius: 1 } }}
      >
        Preskoči na sadržaj
      </Link>

      <Drawer
        variant={desktop ? 'permanent' : 'temporary'}
        open={desktop || mobileOpen}
        onClose={() => setMobileOpen(false)}
        ModalProps={{ keepMounted: true }}
        sx={{
          width: desktop ? sidebarWidth : 0,
          flexShrink: 0,
          transition: theme.transitions.create('width', { duration: theme.transitions.duration.shorter }),
          '& .MuiDrawer-paper': {
            width: desktop ? sidebarWidth : WIDTH_FULL,
            boxSizing: 'border-box',
            overflowX: 'hidden',
            borderRight: 1,
            borderColor: 'divider',
            backgroundColor: 'background.paper',
            transition: theme.transitions.create('width', { duration: theme.transitions.duration.shorter }),
          },
        }}
      >
        {sidebar}
      </Drawer>

      <Box sx={{ flexGrow: 1, minWidth: 0, display: 'flex', flexDirection: 'column' }}>
        <Box
          component="header"
          sx={{
            position: 'sticky',
            top: 0,
            zIndex: theme.zIndex.appBar,
            height: 64,
            px: { xs: 1.5, md: 3 },
            display: 'flex',
            alignItems: 'center',
            gap: 1,
            borderBottom: 1,
            borderColor: 'divider',
            backgroundColor: (t) => (t.palette.mode === 'light' ? 'rgba(255,255,255,0.85)' : 'rgba(18,26,45,0.85)'),
            backdropFilter: 'blur(10px)',
          }}
        >
          {!desktop && (
            <IconButton edge="start" onClick={() => setMobileOpen(true)} aria-label="Otvori izbornik">
              <MenuIcon />
            </IconButton>
          )}
          <Box sx={{ flex: 1 }} />
          <Tooltip title={mode === 'light' ? 'Tamna tema' : 'Svijetla tema'}>
            <IconButton onClick={toggle} aria-label={mode === 'light' ? 'Prebaci na tamnu temu' : 'Prebaci na svijetlu temu'}>
              {mode === 'light' ? <DarkModeOutlinedIcon /> : <LightModeOutlinedIcon />}
            </IconButton>
          </Tooltip>
          <UserMenu />
        </Box>

        <Box component="main" id="main" tabIndex={-1} sx={{ flex: 1, px: { xs: 2, md: 4 }, py: 3, maxWidth: 1600, width: '100%', mx: 'auto', outline: 'none' }}>
          {children}
        </Box>
      </Box>

    </Box>
  );
}
