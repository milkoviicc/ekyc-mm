import FilterListIcon from '@mui/icons-material/FilterList';
import { Badge, Box, Button, Chip, Collapse, Paper, Stack } from '@mui/material';
import { ReactNode, useState } from 'react';
import FilterGrid from './FilterGrid';

export type ActiveFilter = { key: string; label: string; onDelete: () => void };

type Props = {
  /** The filter inputs. */
  children: ReactNode;
  /** Filters currently applied, shown as removable chips even when the panel is collapsed. */
  active: ActiveFilter[];
  onApply: () => void;
  onClear: () => void;
  /** Extra controls on the right (e.g. refresh). */
  actions?: ReactNode;
  defaultOpen?: boolean;
};

/**
 * Collapsible filter panel: closed by default so the data is the focus, with applied filters visible as chips
 * (one click to remove) so the user always knows why a list is short.
 */
export default function FilterBar({ children, active, onApply, onClear, actions, defaultOpen = false }: Props) {
  const [open, setOpen] = useState(defaultOpen);

  return (
    <Paper variant="outlined" sx={{ mb: 2, p: 1.5 }}>
      <Stack direction="row" spacing={1} useFlexGap sx={{ alignItems: "center", flexWrap: "wrap" }}>
        <Badge color="primary" badgeContent={active.length} invisible={active.length === 0}>
          <Button
            variant={open ? 'contained' : 'outlined'}
            color={open ? 'primary' : 'inherit'}
            startIcon={<FilterListIcon />}
            onClick={() => setOpen((v) => !v)}
            aria-expanded={open}
            sx={{ borderColor: 'divider' }}
          >
            Filteri
          </Button>
        </Badge>
        {active.map((f) => (
          <Chip key={f.key} label={f.label} onDelete={f.onDelete} variant="outlined" />
        ))}
        {active.length > 0 && (
          <Button size="small" onClick={onClear}>
            Očisti sve
          </Button>
        )}
        <Box sx={{ flex: 1 }} />
        {actions}
      </Stack>
      <Collapse in={open} unmountOnExit>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            onApply();
          }}
        >
          <FilterGrid sx={{ mt: 2 }}>{children}</FilterGrid>
          <Stack direction="row" spacing={1.5} sx={{ mt: 2 }}>
            <Button type="submit" variant="contained">
              Primijeni
            </Button>
            <Button variant="text" onClick={onClear}>
              Isprazni
            </Button>
          </Stack>
        </form>
      </Collapse>
    </Paper>
  );
}
