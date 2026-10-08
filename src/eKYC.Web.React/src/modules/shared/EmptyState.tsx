import InboxOutlinedIcon from '@mui/icons-material/InboxOutlined';
import { Box, Typography } from '@mui/material';
import { ReactElement, ReactNode } from 'react';

type Props = {
  title: string;
  description?: string;
  icon?: ReactElement;
  action?: ReactNode;
};

/** Friendly empty/zero-result block - used instead of a bare "no data" line. */
export default function EmptyState({ title, description, icon = <InboxOutlinedIcon />, action }: Props) {
  return (
    <Box sx={{ textAlign: 'center', py: 6, px: 2, color: 'text.secondary' }}>
      <Box
        sx={{
          width: 56,
          height: 56,
          borderRadius: '50%',
          mx: 'auto',
          mb: 2,
          display: 'grid',
          placeItems: 'center',
          bgcolor: 'action.hover',
          '& svg': { fontSize: 28 },
        }}
      >
        {icon}
      </Box>
      <Typography variant="subtitle1" color="text.primary">
        {title}
      </Typography>
      {description && (
        <Typography variant="body2" sx={{ mt: 0.5, maxWidth: 420, mx: 'auto' }}>
          {description}
        </Typography>
      )}
      {action && <Box sx={{ mt: 2 }}>{action}</Box>}
    </Box>
  );
}
