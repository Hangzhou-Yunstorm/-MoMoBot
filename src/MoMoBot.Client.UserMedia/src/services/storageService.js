export class StorageService {
    storage = null;

    constructor(session = false) {
        this.storage = session ? sessionStorage : localStorage;
    }

    retrieve(key, defValue = null) {
        let item = this.storage.getItem(key);

        if (item && item !== '') {
            return JSON.parse(item);
        }
        return defValue;
    }

    store(key, value) {
        this.storage.setItem(key, JSON.stringify(value));
    }
}

export const storeService = new StorageService();
export const sessionStoreService = new StorageService(true);