import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import { Button } from '@mui/material';
import { ReactNode } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { usePermissions } from '../../hooks/usePermissions';
import EmptyState from './EmptyState';

type Props = {
  /** A_Objct.Asmbly_Cd codes; the user needs at least one. */
  codes: readonly string[];
  children: ReactNode;
};

/** Shows the screen only to users whose roles grant it; others get a clear explanation instead of an empty page or 403 errors. */
export default function RequireAccess({ codes, children }: Props) {
  const { canOpen } = usePermissions();

  if (canOpen(codes)) return <>{children}</>;

  return (
    <EmptyState
      icon={<LockOutlinedIcon />}
      title="Nemate pristup ovom ekranu"
      description="Vaša uloga nema pravo otvoriti ovaj dio aplikacije. Obratite se administratoru ako vam je potreban."
      action={
        <Button component={RouterLink} to="/" variant="outlined">
          Na početnu
        </Button>
      }
    />
  );
}
