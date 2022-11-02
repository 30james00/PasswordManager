import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import type { IAccount } from '@/models/accountModels';
import router from '@/router';

export const useAccountStore = defineStore('account', {
  state: () => {
    let localAccount = localStorage.getItem('account');
    return {
      account:
        localAccount != null ? (JSON.parse(localAccount) as IAccount) : null,
    };
  },
  actions: {
    login(account: IAccount): void {
      // update pinia state
      this.account = account;

      // store user details and jwt in local storage to keep user logged in between page refreshes
      localStorage.setItem('account', JSON.stringify(account));
      router.push({ name: 'password-list' });
    },
    logout(): void {
      this.account = null;
      localStorage.removeItem('account');
      router.push({ name: 'login' });
    },
  },
  getters: {
    isLoggedIn(): boolean {
      return this.account != null;
    },
  },
});
