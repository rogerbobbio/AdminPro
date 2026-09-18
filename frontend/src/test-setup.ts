// jsdom (the test environment) does not implement window.matchMedia. ThemeService
// calls it to resolve the OS-level light/dark preference, so any spec that
// instantiates it (directly, or via AppShell) needs this polyfilled first.
if (typeof window !== 'undefined' && !window.matchMedia) {
  window.matchMedia = (query: string): MediaQueryList =>
    ({
      matches: false,
      media: query,
      onchange: null,
      addListener: () => {},
      removeListener: () => {},
      addEventListener: () => {},
      removeEventListener: () => {},
      dispatchEvent: () => false,
    }) as MediaQueryList;
}
