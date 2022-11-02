export interface ISavedPassword {
  id: string;
  webAddress: string;
  description: string;
  login: string;
  accountId: string;
}

export interface ICreatePasswordDto {
  password: string;
  description: string;
  webAddress: string;
  login: string;
}

export interface IEditPasswordDto {
  id: string;
  password: string;
  description: string;
  webAddress: string;
  login: string;
}
