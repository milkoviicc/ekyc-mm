import { Box, Typography } from '@mui/material';
import { ReactNode } from 'react';

type Props = {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
};

export default function PageHeader({ title, subtitle, actions }: Props) {
  return (
    <Box sx={{ mb: 2, display: 'flex', alignItems: 'flex-start', gap: 2 }}>
      <Box sx={{ flex: 1 }}>
        <Typography variant="h4" gutterBottom={Boolean(subtitle)}>
          {title}
        </Typography>
        {subtitle && (
          <Typography variant="body2" color="text.secondary">
            {subtitle}
          </Typography>
        )}
      </Box>
      {actions}
    </Box>
  );
}
