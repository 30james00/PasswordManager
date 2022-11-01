import { createApp } from 'vue';
import { createPinia } from 'pinia';

import App from './App.vue';
import router from './router';
import axios from '@/plugins/axios'
import { useAccountStore } from './stores/counter';
import type { AxiosInstance } from 'axios';

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(axios, {
  baseUrl: 'https://localhost:5001/api',
  token: useAccountStore().$state.account,
})

app.mount('#app');

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $axios: AxiosInstance
  }
}
