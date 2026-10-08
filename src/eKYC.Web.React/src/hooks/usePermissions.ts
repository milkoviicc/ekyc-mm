import { useCallback, useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { getMe } from '../store/me';

/**
 * The signed-in user (GET api/me) and what they may do. The API stays the real gatekeeper; this only decides what to show.
 * While the request is running, or if it failed, nothing is hidden - hiding on error would turn a network blip into
 * an empty app, and the server rejects forbidden calls anyway.
 */
export function usePermissions() {
  const dispatch = useAppDispatch();
  const { me, pending, error } = useAppSelector((state) => state.me);

  useEffect(() => {
    if (me === null && !pending && error === null) dispatch(getMe());
  }, [me, pending, error, dispatch]);

  const ready = me !== null;

  /** True if the user's roles grant at least one of these objects (A_Objct.Asmbly_Cd, e.g. "tabAdmin"). Empty list = no restriction. */
  const canOpen = useCallback(
    (codes?: readonly string[]) => {
      if (!codes || codes.length === 0 || me === null) return true;
      return me.Permissions.some((p) => p.Asmbly_Cd !== null && p.Asmbly_Cd !== undefined && codes.includes(p.Asmbly_Cd));
    },
    [me],
  );

  /** True if the user holds at least one of these roles (UNOS, ODOBR1, ADMIN, ...). */
  const hasRole = useCallback(
    (...roles: string[]) => {
      if (me === null) return true;
      return me.Roles.some((r) => roles.some((wanted) => wanted.toLowerCase() === r.toLowerCase()));
    },
    [me],
  );

  return { me, ready, loading: pending, canOpen, hasRole };
}
