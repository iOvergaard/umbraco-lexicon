export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Lexicon Entrypoint",
    alias: "Lexicon.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
