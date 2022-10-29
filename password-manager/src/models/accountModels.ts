export interface IAccount {
  login: string;
  token: string;
}

export interface ILoginDto {
  login: string;
  password: string;
}

export interface IRegisterDto {
  login: string;
  password: string;
  isPasswordKeptAsHash: boolean;
}

export interface IChangePasswordDto {
  oldPassword: string;
  newPassword: string;
  isPasswordKeptAsHash: boolean;
}
