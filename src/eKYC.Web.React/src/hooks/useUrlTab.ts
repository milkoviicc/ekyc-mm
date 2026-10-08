import { useCallback } from 'react';
import { useSearchParams } from 'react-router-dom';

/**
 * Keeps the selected tab in the URL (?tab=reports), so a tab can be bookmarked/shared and the browser back button works.
 * `keys` are the tab ids in display order; an unknown or missing value selects the first tab.
 */
export function useUrlTab<K extends string>(keys: readonly K[], param = 'tab'): [number, (index: number) => void] {
  const [params, setParams] = useSearchParams();
  const current = keys.indexOf((params.get(param) ?? '') as K);
  const index = current === -1 ? 0 : current;

  const setIndex = useCallback(
    (next: number) => {
      setParams(
        (prev) => {
          const copy = new URLSearchParams(prev);
          if (next === 0) copy.delete(param);
          else copy.set(param, keys[next]);
          return copy;
        },
        { replace: false },
      );
    },
    [keys, param, setParams],
  );

  return [index, setIndex];
}
