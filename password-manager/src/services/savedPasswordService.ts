import type {
  ICreatePasswordDto,
  IEditPasswordDto,
  ISavedPassword,
} from '@/models/savedPasswordModels';
import axios from 'axios';

const CONTROLLER = 'account';

export async function listPasswords(): Promise<Array<ISavedPassword> | null> {
  let responce;
  try {
    responce = await axios.get(`/${CONTROLLER}`);
  } catch (e) {
    console.log('Error getting saved passwords');
    return null;
  }
  return responce.data;
}

export async function createPassword(
  createPasswordDto: ICreatePasswordDto
): Promise<ISavedPassword | null> {
  let responce;
  try {
    responce = await axios.post(`/${CONTROLLER}`, createPasswordDto);
  } catch (e) {
    console.log('Error saving new password');
    return null;
  }
  return responce.data;
}

export async function editPassword(
  editPasswordDto: IEditPasswordDto
): Promise<ISavedPassword | null> {
  let responce;
  try {
    responce = await axios.patch(`/${CONTROLLER}`, editPasswordDto);
  } catch (e) {
    console.log('Error editing password');
    return null;
  }
  return responce.data;
}

export async function deletePassword(id: string): Promise<boolean> {
  let responce;
  try {
    responce = await axios.delete(`/${CONTROLLER}/${id}`);
  } catch (e) {
    console.log('Error deleting saved password');
    return false;
  }
  return true;
}

export async function decryptPassword(id: string): Promise<string | null> {
  let responce;
  try {
    responce = await axios.get(`/${CONTROLLER}/decrypt/${id}`);
  } catch (e) {
    console.log('Error decrypting saved password');
    return null;
  }
  return responce.data;
}
