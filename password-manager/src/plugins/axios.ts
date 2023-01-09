import { useAccountStore } from '@/stores/accountStore';
import axios, {
  AxiosError,
  type AxiosRequestConfig,
  type AxiosResponse,
} from 'axios';
import type { App } from 'vue';
import { useToast } from 'vue-toastification';

interface AxiosOptions {
  baseUrl?: string;
}

const toast = useToast();

const onRequest = (config: AxiosRequestConfig): AxiosRequestConfig => {
  const accoutStore = useAccountStore();
  if (accoutStore.isLoggedIn && config.headers) {
    config.headers['Authorization'] = `Bearer ${accoutStore.account?.token}`;
  }
  return config;
};

const onRequestError = (error: AxiosError): Promise<AxiosError> => {
  console.error(`[request error] [${JSON.stringify(error)}]`);
  return Promise.reject(error);
};

const onResponse = (response: AxiosResponse): AxiosResponse => {
  return response;
};

const onResponseError = (error: AxiosError): Promise<AxiosError> => {
  if (error.response?.status == 400)
    toast.error(error.response?.data as string);
  if (error.response?.status == 401)
    toast.error('You are currently not logged in');
  if (error.response?.status == 403) {
    toast.error(error.response?.data as string);
  }
  if (error.response?.status == 500) toast.error('Server error');
  console.error(`[response error] [${JSON.stringify(error)}]`);
  return Promise.reject(error);
};

export default {
  install: (app: App, options: AxiosOptions) => {
    let localAxios = axios.create({
      baseURL: options.baseUrl,
    });
    localAxios.interceptors.request.use(onRequest, onRequestError);
    localAxios.interceptors.response.use(onResponse, onResponseError);
    app.config.globalProperties.$axios = localAxios;
  },
};
