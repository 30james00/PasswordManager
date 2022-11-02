import { createApp } from 'vue';
import { createPinia } from 'pinia';

import App from './App.vue';
import router from './router';
import axios from '@/plugins/axios'
import { useAccountStore } from './stores/accountStore';
import type { AxiosInstance } from 'axios';
import Toast, { POSITION, type PluginOptions } from "vue-toastification";
import "vue-toastification/dist/index.css";

const app = createApp(App);

app.use(createPinia());

app.use(router);

app.use(axios, {
  baseUrl: 'https://localhost:5001/api',
  token: useAccountStore().account?.token,
})

const toastOptions: PluginOptions = {
  position: POSITION.BOTTOM_CENTER
};
app.use(Toast, toastOptions);

app.mount('#app');

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $axios: AxiosInstance
  }
}
