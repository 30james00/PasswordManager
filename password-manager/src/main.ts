import { createApp } from 'vue';
import { createPinia } from 'pinia';
import 'vue-toastification/dist/index.css';

import App from './App.vue';
import axios from '@/plugins/axios';
import { useAccountStore } from './stores/accountStore';
import type { AxiosInstance } from 'axios';
import Toast, { POSITION, type PluginOptions } from 'vue-toastification';
import router from './router';

import { library } from '@fortawesome/fontawesome-svg-core';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faKey, faPen, faShareNodes, faTrash, faUser } from '@fortawesome/free-solid-svg-icons';

library.add(faUser, faPen, faKey, faShareNodes, faTrash);

const app = createApp(App);

app.use(createPinia());
app.use(router);

const accountStore = useAccountStore();
app.use(axios, {
  baseUrl: 'http://localhost:5000/api',
});

const toastOptions: PluginOptions = {
  position: POSITION.BOTTOM_CENTER,
};
app.use(Toast, toastOptions);
app.component('font-awesome-icon', FontAwesomeIcon);
app.mount('#app');

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $axios: AxiosInstance;
  }
}
