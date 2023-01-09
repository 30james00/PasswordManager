export interface ISharedPassword {
  id: string;
  savedPasswordId: string;
  password: string;
  login: string;
  owner: string;
  webAddress: string;
  description: string;
}

export interface ISharedPasswordEditDeleteDto {
  id: string;
  login: string;
}