import axios from 'axios';

const ENV = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL || '',
};

export const apiInstance = axios.create({
  baseURL: ENV.API_BASE_URL,
});
