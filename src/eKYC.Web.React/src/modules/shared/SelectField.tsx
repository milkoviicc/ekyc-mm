import { FormControl, InputLabel, MenuItem, Select } from '@mui/material';

export type SelectOption<V extends string | number> = { value: V; label: string };

type SingleProps<V extends string | number> = {
  label: string;
  options: SelectOption<V>[];
  value: V | null | undefined;
  onChange: (value: V | null) => void;
  /** Adds an empty "—" entry so the filter can be cleared. */
  clearable?: boolean;
  required?: boolean;
  error?: boolean;
  disabled?: boolean;
};

/** Single-value select that returns the typed value (or null when cleared). */
export function SelectField<V extends string | number>({
  label,
  options,
  value,
  onChange,
  clearable = true,
  required,
  error,
  disabled,
}: SingleProps<V>) {
  return (
    <FormControl size="small" fullWidth required={required} error={error} disabled={disabled}>
      <InputLabel>{label}</InputLabel>
      <Select
        label={label}
        value={value === null || value === undefined ? '' : String(value)}
        onChange={(event) => {
          const raw = event.target.value;
          if (raw === '') return onChange(null);
          const match = options.find((o) => String(o.value) === raw);
          onChange(match ? match.value : null);
        }}
      >
        {clearable && (
          <MenuItem value="">
            <em>—</em>
          </MenuItem>
        )}
        {options.map((o) => (
          <MenuItem key={String(o.value)} value={String(o.value)}>
            {o.label}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}

type MultiProps<V extends string | number> = {
  label: string;
  options: SelectOption<V>[];
  value: V[];
  onChange: (value: V[]) => void;
};

/** Multi-value select, shows the chosen labels comma-separated. */
export function MultiSelectField<V extends string | number>({ label, options, value, onChange }: MultiProps<V>) {
  return (
    <FormControl size="small" fullWidth>
      <InputLabel>{label}</InputLabel>
      <Select
        multiple
        label={label}
        value={value.map(String)}
        onChange={(event) => {
          const raw = event.target.value;
          const chosen = typeof raw === 'string' ? raw.split(',') : raw;
          onChange(options.filter((o) => chosen.includes(String(o.value))).map((o) => o.value));
        }}
        renderValue={(selected) =>
          options
            .filter((o) => selected.includes(String(o.value)))
            .map((o) => o.label)
            .join(', ')
        }
      >
        {options.map((o) => (
          <MenuItem key={String(o.value)} value={String(o.value)}>
            {o.label}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
}
