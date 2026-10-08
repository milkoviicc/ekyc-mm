import { useEffect, useRef, useState } from 'react';
import { A_Object_Locks, AcquireLockRequest } from '../models';
import { ApiError } from '../http-common';
import { useAppDispatch } from '../store/hooks';
import { acquireLock, getLockSettings, heartbeatLock, releaseLock } from '../store/editLocks';

export type EditLockStatus =
  /** Not editing (request is null). */
  | 'idle'
  | 'acquiring'
  /** The caller holds the lock - safe to edit. */
  | 'held'
  /** Someone else is editing this record; see `holder`. */
  | 'blocked'
  /** The lock was held but is gone (expired and taken over, or released by an administrator). */
  | 'lost'
  /** The lock service failed for another reason (e.g. no matching user); editing is not blocked by it. */
  | 'unavailable';

export type EditLock = {
  status: EditLockStatus;
  /** The lock that is in the way (status "blocked"). */
  holder: A_Object_Locks | null;
  message: string | null;
};

const DEFAULT_HEARTBEAT_SECONDS = 120;

/**
 * Pessimistic edit lock (Pattern 2) for the lifetime of an open editor.
 * Pass the record while the editor is open and `null` otherwise: the hook takes the lock, refreshes it periodically and
 * releases it on close. If someone else already holds the record, `status` is "blocked" and the editor should be read-only.
 * `row_version` (Pattern 1) still protects the save itself, so "unavailable" does not stop editing.
 */
export function useEditLock(request: AcquireLockRequest | null): EditLock {
  const dispatch = useAppDispatch();
  const [state, setState] = useState<EditLock>({ status: 'idle', holder: null, message: null });
  const lockIdRef = useRef<number | null>(null);

  // A stable key, so a new object with the same values does not restart the lock.
  const key = request ? `${request.Object_Class}#${request.Object_Id}` : null;
  const requestRef = useRef(request);
  requestRef.current = request;

  useEffect(() => {
    const current = requestRef.current;
    if (key === null || current === null) {
      setState({ status: 'idle', holder: null, message: null });
      return;
    }

    let cancelled = false;
    let timer: ReturnType<typeof setInterval> | undefined;
    setState({ status: 'acquiring', holder: null, message: null });

    (async () => {
      try {
        const lock = await dispatch(acquireLock(current)).unwrap();
        if (cancelled) {
          // The editor closed while the request was in flight - give the lock straight back.
          dispatch(releaseLock(lock.Object_Lock_Id));
          return;
        }
        lockIdRef.current = lock.Object_Lock_Id;
        setState({ status: 'held', holder: null, message: null });

        let seconds = DEFAULT_HEARTBEAT_SECONDS;
        try {
          seconds = (await dispatch(getLockSettings()).unwrap()).HeartbeatSeconds || seconds;
        } catch {
          /* keep the default */
        }
        if (cancelled) return;

        timer = setInterval(async () => {
          try {
            await dispatch(heartbeatLock(lock.Object_Lock_Id)).unwrap();
          } catch (e) {
            if ((e as ApiError).status === 409) {
              clearInterval(timer);
              lockIdRef.current = null;
              setState({ status: 'lost', holder: null, message: 'Zaključavanje je isteklo ili ga je oslobodio administrator.' });
            }
          }
        }, Math.max(seconds, 10) * 1000);
      } catch (e) {
        if (cancelled) return;
        const error = e as ApiError;
        if (error.status === 409 && error.currentRecord) {
          setState({ status: 'blocked', holder: error.currentRecord as A_Object_Locks, message: error.message });
        } else {
          setState({ status: 'unavailable', holder: null, message: error.message });
        }
      }
    })();

    return () => {
      cancelled = true;
      if (timer) clearInterval(timer);
      const id = lockIdRef.current;
      lockIdRef.current = null;
      if (id !== null) dispatch(releaseLock(id));
    };
  }, [key, dispatch]);

  return state;
}
