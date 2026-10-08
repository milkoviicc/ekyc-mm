import { Box, Breadcrumbs, Link, Typography } from '@mui/material';
import { ReactNode } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { APP_ROUTES } from '../../utils/routes';

type Props = {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
};

/** Page title + one-line purpose, a breadcrumb back to the dashboard, and optional primary actions on the right. */
export default function PageHeader({ title, subtitle, actions }: Props) {
  const isHome = APP_ROUTES[0].label === title;

  return (
    <Box sx={{ mb: 3, display: 'flex', alignItems: 'flex-end', gap: 2, flexWrap: 'wrap' }}>
      <Box sx={{ flex: 1, minWidth: 240 }}>
        {!isHome && (
          <Breadcrumbs aria-label="Putanja" sx={{ mb: 0.5, fontSize: '0.8125rem' }}>
            <Link component={RouterLink} to="/" underline="hover" color="text.secondary">
              Nadzorna ploča
            </Link>
            <Typography variant="body2" color="text.primary" component="span">
              {title}
            </Typography>
          </Breadcrumbs>
        )}
        <Typography variant="h4" component="h1">
          {title}
        </Typography>
        {subtitle && (
          <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5, maxWidth: 760 }}>
            {subtitle}
          </Typography>
        )}
      </Box>
      {actions && <Box sx={{ display: 'flex', gap: 1 }}>{actions}</Box>}
    </Box>
  );
}
