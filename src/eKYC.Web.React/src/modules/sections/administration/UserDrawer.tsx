import AddIcon from '@mui/icons-material/Add';
import CloseIcon from '@mui/icons-material/Close';
import {
  Alert,
  Box,
  Button,
  Chip,
  Divider,
  Drawer,
  FormControlLabel,
  IconButton,
  Stack,
  Switch,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import dayjs, { Dayjs } from 'dayjs';
import { useState } from 'react';
import { A_ISRole, A_Usr, A_Usr_ISRole } from '../../../models';
import { toApiDate, formatDate } from '../../../utils/formatting';
import { SelectField } from '../../shared/SelectField';

export type UserForm = Pick<A_Usr, 'Lgn_Nm' | 'Usr_Nm_Fst' | 'Usr_Nm_Lst' | 'Email' | 'Usr_St'>;

type Props = {
  /** The user to edit, or null to create a new one. */
  user: A_Usr | null;
  open: boolean;
  roles: A_ISRole[];
  /** All assignments of this user (active and ended). */
  assignments: A_Usr_ISRole[];
  saving: boolean;
  /** Message from a failed save (validation, duplicate login, ...). */
  error: string | null;
  onClose: () => void;
  onSave: (form: UserForm) => void;
  onAssign: (roleId: number, from: string | null, to: string | null) => void;
  onRevoke: (assignment: A_Usr_ISRole) => void;
};

const isActive = (a: A_Usr_ISRole) => !a.Vld_To_Dt || dayjs(a.Vld_To_Dt).isAfter(dayjs());

/**
 * Side panel to create or edit a user and manage the roles they hold. Remounted per user (see key in UsersTab), so the
 * form always starts from the stored row. Users are never deleted - "Aktivan" off deactivates (Usr_St = I).
 */
export default function UserDrawer({ user, open, roles, assignments, saving, error, onClose, onSave, onAssign, onRevoke }: Props) {
  const [login, setLogin] = useState(user?.Lgn_Nm ?? '');
  const [first, setFirst] = useState(user?.Usr_Nm_Fst ?? '');
  const [last, setLast] = useState(user?.Usr_Nm_Lst ?? '');
  const [email, setEmail] = useState(user?.Email ?? '');
  const [active, setActive] = useState((user?.Usr_St ?? 'V') === 'V');
  const [submitted, setSubmitted] = useState(false);

  const [newRole, setNewRole] = useState<number | null>(null);
  const [newFrom, setNewFrom] = useState<Dayjs | null>(null);
  const [newTo, setNewTo] = useState<Dayjs | null>(null);

  const loginMissing = login.trim().length === 0;
  const loginTooLong = login.trim().length > 20;

  const submit = () => {
    setSubmitted(true);
    if (loginMissing || loginTooLong) return;
    onSave({ Lgn_Nm: login.trim(), Usr_Nm_Fst: first.trim() || null, Usr_Nm_Lst: last.trim() || null, Email: email.trim() || null, Usr_St: active ? 'V' : 'I' });
  };

  const heldRoleIds = new Set(assignments.filter(isActive).map((a) => a.ISRol_Id));
  const availableRoles = roles.filter((r) => r.ISRol_St === 'V' && !heldRoleIds.has(r.ISRol_Id));
  const roleName = (id?: number | null) => roles.find((r) => r.ISRol_Id === id)?.ISRol_Nm ?? `#${id}`;

  const addRole = () => {
    if (newRole === null) return;
    onAssign(newRole, toApiDate(newFrom), toApiDate(newTo));
    setNewRole(null);
    setNewFrom(null);
    setNewTo(null);
  };

  return (
    <Drawer anchor="right" open={open} onClose={onClose} slotProps={{ paper: { sx: { width: { xs: '100%', sm: 480 } } } }}>
      <Stack direction="row" sx={{ p: 2.5, alignItems: 'center' }}>
        <Typography variant="h5" sx={{ flex: 1 }}>
          {user ? `Korisnik ${user.Lgn_Nm}` : 'Novi korisnik'}
        </Typography>
        <IconButton onClick={onClose} aria-label="Zatvori">
          <CloseIcon />
        </IconButton>
      </Stack>
      <Divider />

      <Box sx={{ p: 2.5, overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: 2 }}>
        {error && <Alert severity="error">{error}</Alert>}

        <TextField
          label="Korisničko ime (prijava)"
          required
          value={login}
          onChange={(e) => setLogin(e.target.value)}
          error={submitted && (loginMissing || loginTooLong)}
          helperText={
            submitted && loginMissing
              ? 'Korisničko ime je obvezno.'
              : submitted && loginTooLong
                ? 'Najviše 20 znakova.'
                : 'Mora odgovarati imenu Windows računa (bez domene).'
          }
          slotProps={{ htmlInput: { maxLength: 20 } }}
        />
        <Stack direction="row" spacing={2}>
          <TextField label="Ime" fullWidth value={first} onChange={(e) => setFirst(e.target.value)} slotProps={{ htmlInput: { maxLength: 40 } }} />
          <TextField label="Prezime" fullWidth value={last} onChange={(e) => setLast(e.target.value)} slotProps={{ htmlInput: { maxLength: 40 } }} />
        </Stack>
        <TextField label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} slotProps={{ htmlInput: { maxLength: 50 } }} />
        <FormControlLabel
          control={<Switch checked={active} onChange={(e) => setActive(e.target.checked)} />}
          label={active ? 'Aktivan' : 'Neaktivan (ne može se prijaviti)'}
        />
        <Box>
          <Button variant="contained" onClick={submit} disabled={saving}>
            {user ? 'Spremi promjene' : 'Kreiraj korisnika'}
          </Button>
        </Box>

        {user ? (
          <>
            <Divider />
            <Typography variant="h6">Uloge</Typography>
            {assignments.length === 0 && (
              <Typography variant="body2" color="text.secondary">
                Korisnik nema nijednu ulogu.
              </Typography>
            )}
            <Stack spacing={1}>
              {assignments.map((a) => (
                <Box
                  key={a.Usr_ISRol_Id}
                  sx={{ display: 'flex', alignItems: 'center', gap: 1, p: 1, border: 1, borderColor: 'divider', borderRadius: 2, opacity: isActive(a) ? 1 : 0.6 }}
                >
                  <Chip size="small" color={isActive(a) ? 'primary' : 'default'} variant={isActive(a) ? 'filled' : 'outlined'} label={roleName(a.ISRol_Id)} />
                  <Typography variant="caption" color="text.secondary" sx={{ flex: 1 }}>
                    {formatDate(a.Vld_From_Dt)} – {a.Vld_To_Dt ? formatDate(a.Vld_To_Dt) : 'bez isteka'}
                  </Typography>
                  {isActive(a) ? (
                    <Tooltip title="Ukini ulogu (završava danas, zapis ostaje u povijesti)">
                      <Button size="small" color="error" onClick={() => onRevoke(a)}>
                        Ukini
                      </Button>
                    </Tooltip>
                  ) : (
                    <Typography variant="caption">završena</Typography>
                  )}
                </Box>
              ))}
            </Stack>

            <Typography variant="subtitle2" sx={{ mt: 1 }}>
              Dodaj ulogu
            </Typography>
            <SelectField
              label="Uloga"
              clearable={false}
              value={newRole}
              onChange={setNewRole}
              options={availableRoles.map((r) => ({ value: r.ISRol_Id, label: r.ISRol_Nm ?? String(r.ISRol_Id) }))}
            />
            <Stack direction="row" spacing={2}>
              <DatePicker label="Vrijedi od" value={newFrom} onChange={setNewFrom} slotProps={{ textField: { size: 'small', fullWidth: true, helperText: 'Prazno = od sada' }, field: { clearable: true } }} />
              <DatePicker label="Vrijedi do" value={newTo} onChange={setNewTo} slotProps={{ textField: { size: 'small', fullWidth: true, helperText: 'Prazno = bez isteka' }, field: { clearable: true } }} />
            </Stack>
            <Box>
              <Button variant="outlined" startIcon={<AddIcon />} onClick={addRole} disabled={newRole === null}>
                Dodaj ulogu
              </Button>
            </Box>
          </>
        ) : (
          <Typography variant="body2" color="text.secondary">
            Uloge se mogu dodijeliti nakon što se korisnik kreira.
          </Typography>
        )}
      </Box>
    </Drawer>
  );
}
