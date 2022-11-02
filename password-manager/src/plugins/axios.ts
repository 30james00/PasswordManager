import axios, { AxiosError, type AxiosResponse } from 'axios';
import type { App } from 'vue';
import { useToast } from 'vue-toastification';

interface AxiosOptions {
  baseUrl?: string;
  token?: string;
}

const toast = useToast();

const onResponse = (response: AxiosResponse): AxiosResponse => {
  return response;
};

const onResponseError = (error: AxiosError): Promise<AxiosError> => {
  if (error.response?.status == 401)
    toast.error('You are currently not logged in');
  if (error.response?.status == 403) toast.error('Wrong credentials');
  if (error.response?.status == 500) toast.error('Server error');
  console.error(`[response error] [${JSON.stringify(error)}]`);
  return Promise.reject(error);
};

export default {
  install: (app: App, options: AxiosOptions) => {
    let localAxios = axios.create({
      baseURL: options.baseUrl,
      headers: {
        Authorization: options.token ? `Bearer ${options.token}` : '',
      },
    });
    localAxios.interceptors.response.use(onResponse, onResponseError);
    app.config.globalProperties.$axios = localAxios;
  },
};
