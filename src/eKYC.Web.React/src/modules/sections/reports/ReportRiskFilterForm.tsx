import { Box, Button, Paper } from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import dayjs from 'dayjs';
import { ClientType, ReportRiskFilter, RiskEstimate } from '../../../models';
import { toApiDate } from '../../../utils/formatting';
import FilterGrid from '../../shared/FilterGrid';
import { MultiSelectField } from '../../shared/SelectField';

type Props = {
  filter: ReportRiskFilter;
  onChange: (filter: ReportRiskFilter) => void;
  clientTypes: ClientType[] | null;
  riskEstimates: RiskEstimate[] | null;
  onApply: (filter: ReportRiskFilter) => void;
};

/** Date-range + client-type + risk-level filter shared by the "za razdoblje" and "za dan" panels (TkPanelReportsFilter01). */
export default function ReportRiskFilterForm({ filter, onChange, clientTypes, riskEstimates, onApply }: Props) {
  const datePicker = (label: string, key: 'AddDtFrom' | 'AddDtTo' | 'MdfDtFrom' | 'MdfDtTo') => (
    <DatePicker
      label={label}
      value={filter[key] ? dayjs(filter[key]) : null}
      onChange={(value) => onChange({ ...filter, [key]: toApiDate(value) })}
      slotProps={{ textField: { size: 'small', fullWidth: true }, field: { clearable: true } }}
    />
  );

  const clear = () => {
    const empty: ReportRiskFilter = {};
    onChange(empty);
    onApply(empty);
  };

  return (
    <Paper sx={{ p: 2, mb: 2 }} elevation={1}>
      <FilterGrid>
        {datePicker('Ndnk. od', 'AddDtFrom')}
        {datePicker('Ndnk. do', 'AddDtTo')}
        {datePicker('Ndnk. akt. od', 'MdfDtFrom')}
        {datePicker('Ndnk. akt. do', 'MdfDtTo')}
        <MultiSelectField
          label="Vrsta klijenta"
          value={filter.ClntTypCds ?? []}
          onChange={(value) => onChange({ ...filter, ClntTypCds: value })}
          options={(clientTypes ?? []).map((ct) => ({ value: ct.ClntTypCd, label: ct.ClntTypDspn ?? ct.ClntTypCd }))}
        />
        <MultiSelectField
          label="Rizičnost"
          value={filter.RiskEstIds ?? []}
          onChange={(value) => onChange({ ...filter, RiskEstIds: value })}
          options={(riskEstimates ?? []).map((re) => ({ value: re.RskEstId, label: re.RiskLevel ?? String(re.RskEstId) }))}
        />
      </FilterGrid>
      <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
        <Button variant="contained" onClick={() => onApply(filter)}>
          Primjeni filtar
        </Button>
        <Button variant="outlined" onClick={clear}>
          Isprazni filtar
        </Button>
      </Box>
    </Paper>
  );
}
