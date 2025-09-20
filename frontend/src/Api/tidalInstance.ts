import axios from 'axios';
import { ENV } from './env';

export const backendInstance = axios.create({
  baseURL: ENV.BACKEND_LINK || '',
});
