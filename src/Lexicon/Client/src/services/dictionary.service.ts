import { LexiconService } from "../api/index.js";

export class DictionaryService {
  private static instance: DictionaryService;
  private cache = new Map<string, Record<string, string>>();
  private loaded = false;
  private loading?: Promise<void>;

  static getInstance() {
    return this.instance ??= new DictionaryService();
  }

  async getValue(alias: string): Promise<Record<string, string> | undefined> {
    if (!this.loaded) {
      await this.loadAll();
    }
    return this.cache.get(alias);
  }

  private async loadAll() {
    if (this.loading) return this.loading;

    this.loading = this._fetchAllPages();
    await this.loading;
    this.loaded = true;
  }

  private async _fetchAllPages() {
    const {data, error} = await LexiconService.getDictionaryEntries({
      credentials: 'include',
      headers: {
        'Authorization': 'Bearer [redacted]',
      }
    });

    if (error) {
      throw error;
    }

    data?.forEach(entry => {
      this.cache.set(entry.key, entry.translations);
    });
  }
}

export const dictionaryService = DictionaryService.getInstance();
