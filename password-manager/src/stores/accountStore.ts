import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import type { IAccount } from '@/models/accountModels';

export const useAccountStore = defineStore('account', {
  state: () => {
    let localAccount = localStorage.getItem('account');
    return {
      account:
        localAccount != null ? (JSON.parse(localAccount) as IAccount) : null,
    };
  },
  actions: {
    login(account: IAccount) {
      // update pinia state
      this.account = account;

      // store user details and jwt in local storage to keep user logged in between page refreshes
      localStorage.setItem('account', JSON.stringify(account));
    },
    logout() {
      this.account = null;
      localStorage.removeItem('account');
    },
  },
});
