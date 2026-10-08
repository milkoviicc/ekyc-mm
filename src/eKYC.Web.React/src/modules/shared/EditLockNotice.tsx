import { Alert, LinearProgress } from '@mui/material';
import { EditLock } from '../../hooks/useEditLock';
import { formatDuration, fullName } from '../../utils/formatting';

/** True when the form should be read-only: someone else holds the record, or this user's own lock was lost. */
export const editLockBlocks = (lock: EditLock): boolean => lock.status === 'blocked' || lock.status === 'lost';

/** Banner for the state of the edit lock at the top of an edit dialog; renders nothing in the normal "held" case. */
export default function EditLockNotice({ lock }: { lock: EditLock }) {
  switch (lock.status) {
    case 'acquiring':
      return <LinearProgress aria-label="Zaključavanje zapisa" />;
    case 'blocked': {
      const holder = lock.holder;
      const who = holder ? `${fullName(holder.Usr_Nm_Fst, holder.Usr_Nm_Lst) || holder.Lgn_Nm} (${holder.Lgn_Nm})` : 'drugi korisnik';
      return (
        <Alert severity="warning">
          Ovaj zapis trenutno uređuje {who}
          {holder?.Age_Seconds != null ? `, aktivan prije ${formatDuration(holder.Age_Seconds)}` : ''}. Možete ga pregledati, ali ne i spremiti
          izmjene dok korisnik ne završi.
        </Alert>
      );
    }
    case 'lost':
      return <Alert severity="error">{lock.message ?? 'Zaključavanje je izgubljeno.'} Zatvorite prozor i ponovno ga otvorite kako biste nastavili uređivati.</Alert>;
    case 'unavailable':
      return (
        <Alert severity="info">
          Zaključavanje zapisa nije dostupno ({lock.message}). Uređivanje je moguće, a istovremene izmjene i dalje se otkrivaju pri spremanju.
        </Alert>
      );
    default:
      return null;
  }
}
