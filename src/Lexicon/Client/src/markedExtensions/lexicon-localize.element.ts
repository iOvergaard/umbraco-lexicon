import { customElement, html, property, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { dictionaryService } from "../services/dictionary.service.js";

@customElement('ufm-lexicon-localize')
export default class UfmLexiconLocalizeElement extends UmbLitElement {
  @property({ type: String })
  alias!: string;

  @state()
  private _value?: string;

  override async connectedCallback() {
    super.connectedCallback();
    await this.#lookup();
  }

  async #lookup() {
    // 1. Try dictionary (exact match)
    const translations = await dictionaryService.getValue(this.alias);
    if (translations) {
      // Get user's current language - use document lang or fallback
      const lang = document.documentElement.lang || 'en-US';
      // Try exact match, then base language (e.g., 'en' from 'en-US'), then first available
      this._value = translations[lang] ?? translations[lang.split('-')[0]] ?? Object.values(translations)[0];
      return;
    }

    // 2. Fall back to language files (exact match on alias)
    const localized = this.localize.term(this.alias);
    if (localized && localized !== this.alias) {
      this._value = localized;
      return;
    }

    // 3. No translation found - show the alias as fallback
    this._value = this.alias;
  }

  render() {
    return html`${this._value ?? this.alias}`;
  }
}

declare global {
  interface HTMLElementTagNameMap {
    'ufm-lexicon-localize': UfmLexiconLocalizeElement;
  }
}
