export interface SavedPassword {
  id: string;
  webAddress: string;
  description: string;
  login: string;
  accountId: string;
}

export interface CreatePasswordDto {
  password: string;
  description: string;
  login: string;
}

export interface EditPasswordDto {
  id: string;
  password: string;
  description: string;
  login: string;
}
