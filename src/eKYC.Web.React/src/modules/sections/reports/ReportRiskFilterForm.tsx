import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import dayjs from 'dayjs';
import { useState } from 'react';
import { ClientType, ReportRiskFilter, RiskEstimate } from '../../../models';
import { formatDate, toApiDate } from '../../../utils/formatting';
import FilterBar, { ActiveFilter } from '../../shared/FilterBar';
import { MultiSelectField } from '../../shared/SelectField';

type DateKey = 'AddDtFrom' | 'AddDtTo' | 'MdfDtFrom' | 'MdfDtTo';

const DATE_FIELDS: { key: DateKey; label: string }[] = [
  { key: 'AddDtFrom', label: 'Unos od' },
  { key: 'AddDtTo', label: 'Unos do' },
  { key: 'MdfDtFrom', label: 'Izmjena od' },
  { key: 'MdfDtTo', label: 'Izmjena do' },
];

type Props = {
  clientTypes: ClientType[] | null;
  riskEstimates: RiskEstimate[] | null;
  onApply: (filter: ReportRiskFilter) => void;
};

/** Date-range + client-type + risk-level filter shared by the "za razdoblje" and "za dan" panels (TkPanelReportsFilter01). */
export default function ReportRiskFilterForm({ clientTypes, riskEstimates, onApply }: Props) {
  const [draft, setDraft] = useState<ReportRiskFilter>({});
  const [applied, setApplied] = useState<ReportRiskFilter>({});

  const apply = (filter: ReportRiskFilter) => {
    setDraft(filter);
    setApplied(filter);
    onApply(filter);
  };

  const active: ActiveFilter[] = [];
  for (const { key, label } of DATE_FIELDS) {
    const value = applied[key];
    if (value) active.push({ key, label: `${label}: ${formatDate(value)}`, onDelete: () => apply({ ...applied, [key]: null }) });
  }
  if (applied.ClntTypCds?.length) {
    const names = applied.ClntTypCds.map((c) => clientTypes?.find((t) => t.ClntTypCd === c)?.ClntTypDspn ?? c).join(', ');
    active.push({ key: 'types', label: `Vrsta: ${names}`, onDelete: () => apply({ ...applied, ClntTypCds: null }) });
  }
  if (applied.RiskEstIds?.length) {
    const names = applied.RiskEstIds.map((id) => riskEstimates?.find((r) => r.RskEstId === id)?.RiskLevel ?? id).join(', ');
    active.push({ key: 'risk', label: `Rizičnost: ${names}`, onDelete: () => apply({ ...applied, RiskEstIds: null }) });
  }

  return (
    <FilterBar active={active} onApply={() => apply(draft)} onClear={() => apply({})}>
      {DATE_FIELDS.map(({ key, label }) => (
        <DatePicker
          key={key}
          label={label}
          value={draft[key] ? dayjs(draft[key]) : null}
          onChange={(value) => setDraft({ ...draft, [key]: toApiDate(value) })}
          slotProps={{ textField: { size: 'small', fullWidth: true }, field: { clearable: true } }}
        />
      ))}
      <MultiSelectField
        label="Vrsta klijenta"
        value={draft.ClntTypCds ?? []}
        onChange={(value) => setDraft({ ...draft, ClntTypCds: value })}
        options={(clientTypes ?? []).map((ct) => ({ value: ct.ClntTypCd, label: ct.ClntTypDspn ?? ct.ClntTypCd }))}
      />
      <MultiSelectField
        label="Rizičnost"
        value={draft.RiskEstIds ?? []}
        onChange={(value) => setDraft({ ...draft, RiskEstIds: value })}
        options={(riskEstimates ?? []).map((re) => ({ value: re.RskEstId, label: re.RiskLevel ?? String(re.RskEstId) }))}
      />
    </FilterBar>
  );
}
