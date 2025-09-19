import axios from 'axios';
import { ENV } from './env';

export const tidalInstance = axios.create({
  baseURL: ENV.TIDAL_LINK || '',
});
