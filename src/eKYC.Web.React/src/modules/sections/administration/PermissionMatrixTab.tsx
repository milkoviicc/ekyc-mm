import { Alert, Box, Checkbox, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Tooltip, Typography } from '@mui/material';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { A_ISRole, A_Objct } from '../../../models';
import { getObjects, getRoleObjects, getRoles, grantPermission, revokePermission } from '../../../store/admin';
import { useAppDispatch, useAppSelector } from '../../../store/hooks';
import { showErrorMessage, showSuccessMessage } from '../../../store/message';
import ConfirmDialog from '../../shared/ConfirmDialog';
import LoadingBlock from '../../shared/LoadingBlock';

type Pending = { role: A_ISRole; object: A_Objct; grantId: number | null };

/** The "Login" tab is part of every role in the legacy data; removing it would lock the role out completely. */
const isLoginTab = (o: A_Objct) => o.Asmbly_Cd === 'tabLogin';

/**
 * Role x screen matrix: a tick means the role may open that tab (row in A_ISRole_Objct). Clicking a cell grants or removes
 * the right immediately and is written to the audit trail. Removing the admin tab from ADMIN asks for confirmation first,
 * since that would lock every administrator out of this very screen.
 */
export default function PermissionMatrixTab() {
  const dispatch = useAppDispatch();
  const roles = useAppSelector((state) => state.admin.roles);
  const objects = useAppSelector((state) => state.admin.objects);
  const grants = useAppSelector((state) => state.admin.roleObjects);

  const [busy, setBusy] = useState<string | null>(null);
  const [confirm, setConfirm] = useState<Pending | null>(null);

  const reload = useCallback(() => {
    dispatch(getRoles());
    dispatch(getObjects());
    dispatch(getRoleObjects());
  }, [dispatch]);

  useEffect(() => {
    reload();
  }, [reload]);

  const validRoles = useMemo(() => (roles.items ?? []).filter((r) => r.ISRol_St === 'V'), [roles.items]);
  const validObjects = useMemo(() => (objects.items ?? []).filter((o) => o.Objct_St === 'V' && !isLoginTab(o)), [objects.items]);
  const grantId = useCallback(
    (roleId: number, objectId: number) => (grants.items ?? []).find((g) => g.ISRol_Id === roleId && g.Objct_Id === objectId)?.ISRol_Objct_Id ?? null,
    [grants.items],
  );

  const toggle = async ({ role, object, grantId: existing }: Pending) => {
    setBusy(`${role.ISRol_Id}:${object.Objct_Id}`);
    try {
      if (existing === null) {
        await dispatch(grantPermission({ ISRol_Id: role.ISRol_Id, Objct_Id: object.Objct_Id })).unwrap();
        dispatch(showSuccessMessage(`${role.ISRol_Nm}: dodano pravo na „${object.Objct_Dspn ?? object.Objct_Nm}“.`));
      } else {
        await dispatch(revokePermission(existing)).unwrap();
        dispatch(showSuccessMessage(`${role.ISRol_Nm}: oduzeto pravo na „${object.Objct_Dspn ?? object.Objct_Nm}“.`));
      }
    } catch (e) {
      dispatch(showErrorMessage((e as { message: string }).message));
    } finally {
      setBusy(null);
      dispatch(getRoleObjects());
    }
  };

  const onCell = (role: A_ISRole, object: A_Objct) => {
    const pending: Pending = { role, object, grantId: grantId(role.ISRol_Id, object.Objct_Id) };
    const removingAdminFromAdmin = pending.grantId !== null && role.ISRol_Nm === 'ADMIN' && object.Asmbly_Cd === 'tabAdmin';
    if (removingAdminFromAdmin) setConfirm(pending);
    else toggle(pending);
  };

  if (roles.error || objects.error || grants.error) return <Alert severity="error">{roles.error ?? objects.error ?? grants.error}</Alert>;
  if (roles.items === null || objects.items === null || grants.items === null) return <LoadingBlock rows={4} />;

  return (
    <>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Označeno = uloga smije otvoriti taj ekran. Promjena se sprema odmah i bilježi u dnevniku promjena. Korisnici vide promjenu nakon
        što ponovno učitaju aplikaciju.
      </Typography>
      <TableContainer component={Paper} variant="outlined">
        <Table size="small" aria-label="Matrica prava po ulogama">
          <TableHead>
            <TableRow>
              <TableCell sx={{ fontWeight: 650 }}>Uloga</TableCell>
              {validObjects.map((o) => (
                <TableCell key={o.Objct_Id} align="center" sx={{ fontWeight: 650 }}>
                  <Tooltip title={o.Objct_Nm ?? ''}>
                    <span>{o.Objct_Dspn ?? o.Objct_Nm}</span>
                  </Tooltip>
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {validRoles.map((role) => (
              <TableRow key={role.ISRol_Id} hover>
                <TableCell>
                  <Box sx={{ fontWeight: 600 }}>{role.ISRol_Nm}</Box>
                </TableCell>
                {validObjects.map((object) => {
                  const checked = grantId(role.ISRol_Id, object.Objct_Id) !== null;
                  return (
                    <TableCell key={object.Objct_Id} align="center" padding="checkbox">
                      <Checkbox
                        checked={checked}
                        disabled={busy !== null}
                        onChange={() => onCell(role, object)}
                        slotProps={{ input: { 'aria-label': `${role.ISRol_Nm}: ${object.Objct_Dspn ?? object.Objct_Nm}` } }}
                      />
                    </TableCell>
                  );
                })}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <ConfirmDialog
        open={Boolean(confirm)}
        title="Oduzeti pravo administratorima?"
        message="Uloga ADMIN će izgubiti pristup ekranu Administriranje, pa nitko neće moći vratiti pravo kroz aplikaciju. Nastaviti?"
        confirmText="Oduzmi pravo"
        onConfirm={() => {
          const pending = confirm;
          setConfirm(null);
          if (pending) toggle(pending);
        }}
        onCancel={() => setConfirm(null)}
      />
    </>
  );
}
