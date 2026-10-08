import { ButtonBase, Typography } from '@mui/material';
import { alpha, useTheme } from '@mui/material/styles';
import { ReactElement } from 'react';

type Props = {
  label: string;
  value: number | string;
  hint?: string;
  icon: ReactElement;
  color?: 'primary' | 'success' | 'warning' | 'error' | 'info';
  /** When set the card is a toggle button (used as a quick filter). */
  selected?: boolean;
  onClick?: () => void;
};

/** Summary tile. As a quick filter it is a real <button> with aria-pressed, so it is keyboard and screen-reader friendly. */
export default function KpiCard({ label, value, hint, icon, color = 'primary', selected, onClick }: Props) {
  const theme = useTheme();
  const main = theme.palette[color].main;

  return (
    <ButtonBase
      onClick={onClick}
      disabled={!onClick}
      aria-pressed={onClick ? Boolean(selected) : undefined}
      sx={{
        textAlign: 'left',
        display: 'flex',
        alignItems: 'center',
        gap: 1.5,
        p: 2,
        borderRadius: 3,
        border: 1,
        borderColor: selected ? main : 'divider',
        bgcolor: selected ? alpha(main, 0.08) : 'background.paper',
        transition: 'border-color .15s, background-color .15s, transform .15s',
        '&:hover:not(.Mui-disabled)': { borderColor: main, transform: 'translateY(-1px)' },
        '&.Mui-disabled': { opacity: 1 },
        width: '100%',
      }}
    >
      <span
        aria-hidden
        style={{
          width: 40,
          height: 40,
          borderRadius: 12,
          display: 'grid',
          placeItems: 'center',
          background: alpha(main, 0.12),
          color: main,
          flex: 'none',
        }}
      >
        {icon}
      </span>
      <span style={{ minWidth: 0 }}>
        <Typography variant="h5" component="div" sx={{ lineHeight: 1.1 }}>
          {value}
        </Typography>
        <Typography variant="body2" color="text.secondary" noWrap>
          {label}
        </Typography>
        {hint && (
          <Typography variant="caption" color="text.secondary" noWrap component="div">
            {hint}
          </Typography>
        )}
      </span>
    </ButtonBase>
  );
}
