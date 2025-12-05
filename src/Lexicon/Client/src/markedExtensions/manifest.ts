export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "backofficeEntryPoint",
    alias: "Lexicon.Marked.Entrypoint",
    name: "Lexicon Marked Entrypoint",
    js: () => import("./entrypoint.ts")
  },
  {
    type: "markedExtension",
    name: "Lexicon Marked Localize",
    alias: "Lexicon.Marked.Localize",
    js: () => import("./markedExtensions.localize.ts"),
    meta: {
      alias: "lexiconLocalize",
    }
  },
];
