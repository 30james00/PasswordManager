import { ref, computed } from 'vue';
import { defineStore } from 'pinia';
import type { IAccount } from '@/models/accountModels';

export const useAccountStore = defineStore('account', {
  state: () => {
    return {
      account: {} as IAccount,
    };
  },
});
