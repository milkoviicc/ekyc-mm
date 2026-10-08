import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { Alert, Avatar, Box, Button, Chip, Divider, Popover, Skeleton, Stack, Typography } from '@mui/material';
import { MouseEvent, useState } from 'react';
import { usePermissions } from '../../hooks/usePermissions';
import { fullName } from '../../utils/formatting';

const initials = (name: string) =>
  name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((p) => p[0]?.toUpperCase())
    .join('') || '?';

/** Who is signed in (from api/me): name, roles and the screens they can open. */
export default function UserMenu() {
  const { me, ready, loading } = usePermissions();
  const [anchor, setAnchor] = useState<HTMLElement | null>(null);

  if (!ready) return loading ? <Skeleton variant="rounded" width={140} height={36} /> : null;

  const displayName = fullName(me?.User?.Usr_Nm_Fst, me?.User?.Usr_Nm_Lst) || me?.LoginName || 'Korisnik';
  const claimRoles = me?.Roles ?? [];
  return (
    <>
      <Button
        color="inherit"
        onClick={(e: MouseEvent<HTMLElement>) => setAnchor(e.currentTarget)}
        endIcon={<ExpandMoreIcon />}
        aria-haspopup="dialog"
        aria-expanded={Boolean(anchor)}
        sx={{ textTransform: 'none', gap: 0.5, px: 1 }}
      >
        <Avatar sx={{ width: 30, height: 30, fontSize: 13, bgcolor: 'primary.main', mr: 1 }}>{initials(displayName)}</Avatar>
        <Box sx={{ display: { xs: 'none', sm: 'block' }, textAlign: 'left', lineHeight: 1.2 }}>
          <Typography variant="body2" sx={{ fontWeight: 600 }} noWrap>
            {displayName}
          </Typography>
          <Typography variant="caption" color="text.secondary" noWrap component="div">
            {claimRoles.join(' · ') || 'bez uloge'}
          </Typography>
        </Box>
      </Button>

      <Popover
        open={Boolean(anchor)}
        anchorEl={anchor}
        onClose={() => setAnchor(null)}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        slotProps={{ paper: { sx: { width: 340, p: 2, mt: 1 } } }}
      >
        <Typography variant="subtitle1">{displayName}</Typography>
        <Typography variant="body2" color="text.secondary">
          Prijava: {me?.LoginName}
          {me?.User?.Email ? ` · ${me.User.Email}` : ''}
        </Typography>

        {!me?.Matched && (
          <Alert severity="warning" sx={{ mt: 1.5 }}>
            Nema aktivnog korisnika u sustavu za ovu prijavu. Uređivanje zapisa i zaključavanje nisu dostupni.
          </Alert>
        )}

        <Divider sx={{ my: 1.5 }} />
        <Typography variant="overline" color="text.secondary">
          Uloge
        </Typography>
        <Stack direction="row" spacing={0.75} useFlexGap sx={{ flexWrap: 'wrap', mb: 1 }}>
          {claimRoles.length === 0 && <Typography variant="body2">—</Typography>}
          {claimRoles.map((r) => (
            <Chip key={r} size="small" color="primary" variant="outlined" label={r} />
          ))}
        </Stack>

        <Typography variant="overline" color="text.secondary">
          Dostupni ekrani
        </Typography>
        <Stack direction="row" spacing={0.75} useFlexGap sx={{ flexWrap: 'wrap' }}>
          {(me?.Permissions ?? []).map((p) => (
            <Chip key={p.Objct_Id} size="small" label={p.Objct_Dspn ?? p.Objct_Nm} />
          ))}
        </Stack>
      </Popover>
    </>
  );
}
