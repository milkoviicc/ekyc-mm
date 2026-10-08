import '@mui/x-data-grid/themeAugmentation';
import { alpha, createTheme, PaletteMode, ThemeOptions } from '@mui/material/styles';
import { hrHR as coreHrHR } from '@mui/material/locale';
import { hrHR as gridHrHR } from '@mui/x-data-grid/locales';

/** Design tokens. Everything visual (colors, radius, shadows, density) is decided here, not in screens. */
const tokens = {
  light: {
    primary: '#2955E8',
    background: '#F5F7FA',
    paper: '#FFFFFF',
    textPrimary: '#0F172A',
    textSecondary: '#5B6679',
    divider: '#E4E8EF',
    success: '#13854E',
    warning: '#B26A00',
    error: '#C62828',
    info: '#0B73B8',
  },
  dark: {
    primary: '#7DA2FF',
    background: '#0B1120',
    paper: '#121A2D',
    textPrimary: '#E6EAF2',
    textSecondary: '#9AA6BC',
    divider: '#243049',
    success: '#4CC38A',
    warning: '#F0B04A',
    error: '#FF7B72',
    info: '#58B4F0',
  },
} as const;

export const RADIUS = 12;

export function buildTheme(mode: PaletteMode) {
  const t = tokens[mode];

  const options: ThemeOptions = {
    palette: {
      mode,
      primary: { main: t.primary },
      success: { main: t.success },
      warning: { main: t.warning },
      error: { main: t.error },
      info: { main: t.info },
      background: { default: t.background, paper: t.paper },
      text: { primary: t.textPrimary, secondary: t.textSecondary },
      divider: t.divider,
    },
    shape: { borderRadius: RADIUS },
    typography: {
      fontFamily: '"Inter Variable", "Inter", system-ui, -apple-system, "Segoe UI", Roboto, sans-serif',
      h4: { fontSize: '1.625rem', fontWeight: 650, letterSpacing: '-0.02em', lineHeight: 1.25 },
      h5: { fontSize: '1.25rem', fontWeight: 650, letterSpacing: '-0.015em' },
      h6: { fontSize: '1.0625rem', fontWeight: 600, letterSpacing: '-0.01em' },
      subtitle1: { fontWeight: 600 },
      subtitle2: { fontWeight: 600, fontSize: '0.8125rem' },
      body2: { fontSize: '0.875rem' },
      button: { textTransform: 'none', fontWeight: 600, letterSpacing: 0 },
      caption: { letterSpacing: '0.01em' },
    },
    components: {
      MuiCssBaseline: {
        styleOverrides: {
          body: { WebkitFontSmoothing: 'antialiased' },
          // Visible, consistent keyboard focus everywhere.
          ':focus-visible': { outline: `2px solid ${t.primary}`, outlineOffset: 2 },
          '@media (prefers-reduced-motion: reduce)': {
            '*, *::before, *::after': { animationDuration: '0.01ms !important', transitionDuration: '0.01ms !important' },
          },
        },
      },
      MuiPaper: {
        defaultProps: { elevation: 0 },
        styleOverrides: { root: { backgroundImage: 'none' }, outlined: { borderColor: t.divider } },
      },
      MuiButton: {
        defaultProps: { disableElevation: true },
        styleOverrides: { root: { borderRadius: 10, paddingInline: 16 } },
      },
      MuiIconButton: { styleOverrides: { root: { borderRadius: 10 } } },
      MuiTextField: { defaultProps: { size: 'small' } },
      MuiOutlinedInput: { styleOverrides: { root: { borderRadius: 10, backgroundColor: alpha(t.paper, 1) } } },
      MuiChip: { styleOverrides: { root: { fontWeight: 600, borderRadius: 8 } } },
      MuiTab: { styleOverrides: { root: { minHeight: 44, fontWeight: 600 } } },
      MuiTabs: { styleOverrides: { indicator: { height: 3, borderRadius: 3 } } },
      MuiDialog: { styleOverrides: { paper: { borderRadius: 16 } } },
      MuiTooltip: { defaultProps: { arrow: true } },
      MuiListItemButton: { styleOverrides: { root: { borderRadius: 10 } } },
      MuiDataGrid: {
        styleOverrides: {
          root: {
            border: `1px solid ${t.divider}`,
            borderRadius: RADIUS,
            '--DataGrid-t-header-background-base': alpha(t.textPrimary, mode === 'light' ? 0.03 : 0.06),
            '& .MuiDataGrid-columnHeaders': { borderBottom: `1px solid ${t.divider}` },
            '& .MuiDataGrid-columnHeaderTitle': { fontWeight: 650, fontSize: '0.8125rem', color: t.textSecondary },
            '& .MuiDataGrid-cell': { borderColor: alpha(t.divider, 0.7), display: 'flex', alignItems: 'center' },
            '& .MuiDataGrid-columnSeparator': { display: 'none' },
            '& .MuiDataGrid-row:hover': { backgroundColor: alpha(t.primary, 0.05) },
            '& .MuiDataGrid-footerContainer': { borderTop: `1px solid ${t.divider}` },
            // No outline when a cell or header is clicked; keyboard navigation (:focus-visible) keeps a visible ring.
            '& .MuiDataGrid-cell:focus, & .MuiDataGrid-cell:focus-within, & .MuiDataGrid-columnHeader:focus, & .MuiDataGrid-columnHeader:focus-within': {
              outline: 'none !important',
            },
            '& .MuiDataGrid-cell:focus-visible, & .MuiDataGrid-columnHeader:focus-visible': {
              outline: `2px solid ${t.primary} !important`,
              outlineOffset: -2,
            },
          },
        },
      },
    },
  };

  return createTheme(options, coreHrHR, gridHrHR);
}
