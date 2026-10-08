import { Box, Skeleton } from '@mui/material';

type Props = {
  /** Number of placeholder rows. */
  rows?: number;
};

/** Skeleton that looks like a table - shown while a grid's data loads (less jarring than a spinner). */
export default function LoadingBlock({ rows = 6 }: Props) {
  return (
    <Box role="status" aria-label="Učitavanje" sx={{ border: 1, borderColor: 'divider', borderRadius: 3, p: 2, bgcolor: 'background.paper' }}>
      <Skeleton variant="rounded" height={32} sx={{ mb: 1.5 }} />
      {Array.from({ length: rows }, (_, i) => (
        <Skeleton key={i} variant="text" height={34} sx={{ opacity: 1 - i * 0.1 }} />
      ))}
    </Box>
  );
}
