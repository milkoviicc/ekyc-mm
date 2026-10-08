import { createTheme } from '@mui/material/styles';
import { hrHR as coreHrHR } from '@mui/material/locale';
import { hrHR as gridHrHR } from '@mui/x-data-grid/locales';

const theme = createTheme(
  {
    palette: {
      primary: { main: '#1b4f8a' },
      secondary: { main: '#6b7a90' },
      background: { default: '#f4f6f9' },
    },
    shape: { borderRadius: 6 },
    typography: {
      h4: { fontSize: '1.75rem', fontWeight: 500 },
    },
  },
  coreHrHR,
  gridHrHR,
);

export default theme;
