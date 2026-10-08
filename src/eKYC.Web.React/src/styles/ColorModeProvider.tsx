import { CssBaseline, PaletteMode, ThemeProvider, useMediaQuery } from '@mui/material';
import { createContext, ReactNode, useContext, useEffect, useMemo, useState } from 'react';
import { buildTheme } from './theme';

type ColorModeContextValue = {
  mode: PaletteMode;
  toggle: () => void;
};

const ColorModeContext = createContext<ColorModeContextValue>({ mode: 'light', toggle: () => undefined });

const STORAGE_KEY = 'ekyc.colorMode';

const readStored = (): PaletteMode | null => {
  try {
    const value = localStorage.getItem(STORAGE_KEY);
    return value === 'light' || value === 'dark' ? value : null;
  } catch {
    return null; // storage can be blocked (private mode) - fall back to the OS preference
  }
};

/** Light/dark theme: follows the OS until the user picks one, then remembers the choice. */
export function ColorModeProvider({ children }: { children: ReactNode }) {
  const prefersDark = useMediaQuery('(prefers-color-scheme: dark)');
  const [stored, setStored] = useState<PaletteMode | null>(readStored);
  const mode: PaletteMode = stored ?? (prefersDark ? 'dark' : 'light');

  useEffect(() => {
    document.documentElement.style.colorScheme = mode;
  }, [mode]);

  const value = useMemo<ColorModeContextValue>(
    () => ({
      mode,
      toggle: () => {
        const next: PaletteMode = mode === 'light' ? 'dark' : 'light';
        setStored(next);
        try {
          localStorage.setItem(STORAGE_KEY, next);
        } catch {
          /* ignore */
        }
      },
    }),
    [mode],
  );

  const theme = useMemo(() => buildTheme(mode), [mode]);

  return (
    <ColorModeContext.Provider value={value}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
}

export const useColorMode = () => useContext(ColorModeContext);
