import type { MarkedExtension, Tokens } from '@umbraco-cms/backoffice/external/marked';

import './lexicon-localize.element.js';

/**
 * @returns {MarkedExtension} A Marked extension object.
 */
export function lexiconLocalize(): MarkedExtension {
  // Matches #DictionaryKey, #area_key, #area/key, or #DocumentType.Button.Color
  // (?!;) skips HTML entities like &#160;
  const LOCALIZE_PATTERN = /\B#([a-zA-Z_][a-zA-Z0-9_.]*(?:[/_][a-zA-Z0-9_.]+)?)(?!;)/;

  return {
    extensions: [
      {
        name: 'lexiconLocalize',
        level: 'inline',
        start: (src: string) => {
          // Find the first # that could be a localization reference
          const match = src.match(LOCALIZE_PATTERN);
          return match ? src.indexOf(match[0]) : -1;
        },
        tokenizer: (src: string) => {
          // Match from the start of src (Marked slices to start position)
          const match = src.match(/^#([a-zA-Z_][a-zA-Z0-9_.]*(?:[/_][a-zA-Z0-9_.]+)?)(?!;)/);

          console.log('LexiconLocalize tokenizer match:', src, match);

          if (match) {
            const [raw, key] = match;
            return {
              type: 'lexiconLocalize',
              raw: raw,
              tokens: [],
              text: key,
            };
          }

          return undefined;
        },
        renderer(this: unknown, token: Tokens.Generic) {
          console.log('LexiconLocalize renderer:', token);
          return `<ufm-lexicon-localize alias="${token.text}"></ufm-lexicon-localize>`;
        },
      },
    ],
  };
}
