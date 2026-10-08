import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import dayjs, { Dayjs } from 'dayjs';
import { useState } from 'react';
import { CL_Doc_Revisions, RevisionType } from '../../../models';
import { toApiDate } from '../../../utils/formatting';
import { useEditLock } from '../../../hooks/useEditLock';
import EditLockNotice, { editLockBlocks } from '../../shared/EditLockNotice';
import { SelectField } from '../../shared/SelectField';

type Props = {
  open: boolean;
  /** The row to edit (a copy is edited, so cancelling leaves the grid untouched); a blank row for "new". */
  revision: CL_Doc_Revisions;
  isNew: boolean;
  revisionTypes: RevisionType[] | null;
  saving: boolean;
  onCancel: () => void;
  onSave: (revision: CL_Doc_Revisions) => void;
};

/** Create/edit form for one CL_Doc_Revisions row - mirrors TkWindowEditRevision.java's fields. */
export default function RevisionEditDialog(props: Props) {
  // Remounted per open (see key in Revision.tsx) so the form state always starts from the passed revision.
  const { open, isNew, revisionTypes, saving, onCancel, onSave, revision } = props;

  const [typeId, setTypeId] = useState<number | null>(revision.CL_Doc_Typ_Rev_Id || null);
  const [revDate, setRevDate] = useState<Dayjs | null>(revision.Rev_Date ? dayjs(revision.Rev_Date) : null);
  const [rangeFrom, setRangeFrom] = useState<Dayjs | null>(revision.Rev_Range_From ? dayjs(revision.Rev_Range_From) : null);
  const [rangeTo, setRangeTo] = useState<Dayjs | null>(revision.Rev_Range_To ? dayjs(revision.Rev_Range_To) : null);
  const [doneBy, setDoneBy] = useState(revision.Rev_Done_By);
  const [subject, setSubject] = useState(revision.Subject);
  const [recommendation, setRecommendation] = useState(revision.Recommendation ?? '');
  const [submitted, setSubmitted] = useState(false);

  // Pessimistic lock (Pattern 2) while an existing revision is open; a new one has nothing to lock yet.
  const lock = useEditLock(isNew ? null : { Object_Id: revision.CL_Doc_Revision_Id, Object_Class: 'CL_Doc_Revisions', Object_Name: revision.Subject });
  const readOnly = editLockBlocks(lock);

  const typeMissing = typeId === null;
  const dateMissing = revDate === null || !revDate.isValid();
  const doneByMissing = doneBy.trim().length === 0;
  const subjectMissing = subject.trim().length === 0;
  const invalid = typeMissing || dateMissing || doneByMissing || subjectMissing;

  const accept = () => {
    setSubmitted(true);
    if (invalid || typeId === null || revDate === null) return;
    onSave({
      ...revision,
      CL_Doc_Typ_Rev_Id: typeId,
      Rev_Date: toApiDate(revDate) as string,
      Rev_Range_From: toApiDate(rangeFrom),
      Rev_Range_To: toApiDate(rangeTo),
      Rev_Done_By: doneBy.trim(),
      Subject: subject.trim(),
      Recommendation: recommendation.trim() === '' ? null : recommendation.trim(),
    });
  };

  return (
    <Dialog open={open} onClose={onCancel} maxWidth="sm" fullWidth>
      <DialogTitle>{isNew ? 'Nova revizija' : 'Promjena revizije'}</DialogTitle>
      <DialogContent>
        <Box sx={{ mb: 1 }}>
          <EditLockNotice lock={lock} />
        </Box>
        {/* A disabled fieldset disables every native input inside it, including the date pickers. */}
        <Box component="fieldset" disabled={readOnly} sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1, border: 0, m: 0, p: 0, minWidth: 0 }}>
          <SelectField
            label="Vrsta revizije"
            required
            clearable={false}
            error={submitted && typeMissing}
            value={typeId}
            onChange={setTypeId}
            options={(revisionTypes ?? []).map((rt) => ({ value: rt.RevTypeId, label: rt.RevTypeName }))}
          />
          <DatePicker
            label="Nadnevak revizije"
            value={revDate}
            onChange={setRevDate}
            slotProps={{
              textField: {
                size: 'small',
                required: true,
                error: submitted && dateMissing,
                helperText: submitted && dateMissing ? 'Nadnevak revizije je obvezan.' : undefined,
              },
            }}
          />
          <DatePicker label="Vrijedi od" value={rangeFrom} onChange={setRangeFrom} slotProps={{ textField: { size: 'small' }, field: { clearable: true } }} />
          <DatePicker label="Vrijedi do" value={rangeTo} onChange={setRangeTo} slotProps={{ textField: { size: 'small' }, field: { clearable: true } }} />
          <TextField
            size="small"
            label="Revizor"
            required
            multiline
            minRows={2}
            value={doneBy}
            onChange={(e) => setDoneBy(e.target.value)}
            error={submitted && doneByMissing}
            helperText={submitted && doneByMissing ? 'Revizor je obvezan.' : undefined}
            slotProps={{ htmlInput: { maxLength: 150 } }}
          />
          <TextField
            size="small"
            label="Predmet"
            required
            multiline
            minRows={2}
            value={subject}
            onChange={(e) => setSubject(e.target.value)}
            error={submitted && subjectMissing}
            helperText={submitted && subjectMissing ? 'Predmet je obvezan.' : undefined}
            slotProps={{ htmlInput: { maxLength: 150 } }}
          />
          <TextField
            size="small"
            label="Preporuka"
            multiline
            minRows={2}
            value={recommendation}
            onChange={(e) => setRecommendation(e.target.value)}
            slotProps={{ htmlInput: { maxLength: 250 } }}
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel}>Odustanak</Button>
        <Button variant="contained" onClick={accept} disabled={saving || readOnly}>
          Prihvat
        </Button>
      </DialogActions>
    </Dialog>
  );
}
