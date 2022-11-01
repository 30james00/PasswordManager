import type {
  IAccount,
  IChangePasswordDto,
  ILoginDto,
  IRegisterDto,
} from '@/models/accountModels';

import axios from 'axios';

export async function login(loginDto: ILoginDto): Promise<IAccount | null> {
  let responce;
  try {
    responce = await axios.post('/account/login', loginDto);
  } catch (e) {
    console.log('Error loging in');
    return null;
  }
  return responce.data;
}

export async function changePassword(
  changePasswordDto: IChangePasswordDto
): Promise<IAccount | null> {
  let responce;
  try {
    responce = await axios.patch('/account/change-password', changePasswordDto);
  } catch (e) {
    console.log('Error changing MasterPassword');
    return null;
  }
  return responce.data;
}
