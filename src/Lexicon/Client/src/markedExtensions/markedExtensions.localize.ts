
import type { Marked } from '@umbraco-cms/backoffice/external/marked';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbMarkedExtensionApi } from "@umbraco-cms/backoffice/ufm";
import { lexiconLocalize } from "./marked.lexiconLocalize.plugin.js";

export class LexiconLocalizeMarkedExtensionApi implements UmbMarkedExtensionApi {
  constructor(_host: UmbControllerHost, marked: Marked) {
    marked.use(lexiconLocalize());
  }

  destroy() {}
}

export default LexiconLocalizeMarkedExtensionApi;
