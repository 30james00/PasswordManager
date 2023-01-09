import type { ISharedPasswordMiniDto } from './sharedPasswordModels';

export interface ISavedPassword {
  id: string;
  password: string;
  login: string;
  webAddress: string;
  description: string;
  sharedTo: Array<ISharedPasswordMiniDto>;
}

export interface ICreatePasswordDto {
  password: string;
  webAddress: string;
  description: string;
  login: string;
}

export interface IEditPasswordDto {
  id: string;
  login: string;
  password: string;
  webAddress: string;
  description: string;
}
