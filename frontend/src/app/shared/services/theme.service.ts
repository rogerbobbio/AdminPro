import { Injectable, effect, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

const STORAGE_KEY = 'adminpro-theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly theme = signal<ThemeMode>(this.resolveInitialTheme());

  readonly currentTheme = this.theme.asReadonly();

  constructor() {
    effect(() => {
      const mode = this.theme();
      document.documentElement.setAttribute('data-theme', mode);
      localStorage.setItem(STORAGE_KEY, mode);
    });
  }

  toggle(): void {
    this.theme.update((mode) => (mode === 'dark' ? 'light' : 'dark'));
  }

  private resolveInitialTheme(): ThemeMode {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored === 'light' || stored === 'dark') {
      return stored;
    }
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }
}
