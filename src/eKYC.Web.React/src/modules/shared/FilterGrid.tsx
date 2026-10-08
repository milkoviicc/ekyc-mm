import { Box, BoxProps } from '@mui/material';

/** Responsive grid for filter inputs: as many ~220px columns as fit. */
export default function FilterGrid({ children, sx, ...props }: BoxProps) {
  return (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))',
        gap: 2,
        ...sx,
      }}
      {...props}
    >
      {children}
    </Box>
  );
}
