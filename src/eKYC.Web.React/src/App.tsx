import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { CssBaseline, ThemeProvider } from '@mui/material';
import dayjs from 'dayjs';
import 'dayjs/locale/hr';
import { Navigate, Route, Routes } from 'react-router-dom';
import Layout from './modules/layout/Layout';
import MessageManager from './modules/shared/MessageManager';
import theme from './styles/theme';
import { renderAllRoutes } from './utils/routes';

dayjs.locale('hr');

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="hr">
        <Layout>
          <Routes>
            {renderAllRoutes()}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </Layout>
        <MessageManager />
      </LocalizationProvider>
    </ThemeProvider>
  );
}
