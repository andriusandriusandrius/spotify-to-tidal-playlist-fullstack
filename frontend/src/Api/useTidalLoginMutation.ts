import { useMutation } from '@tanstack/react-query';
import { tidalInstance } from './tidalInstance';

export type TidalLoginRequest = {
  client_id: string;
  redirect_uri: string;
  scope: string[];
  code_challange_method: 'S256';
  code_challange: string;
  state?: string;
};
export type TidalLoginResponse = {
  access_token: string;
  expires_in: number;
  refresh_token: string;
};

const login = async (data: TidalLoginRequest) => {
  const response = await tidalInstance.post<TidalLoginResponse>('/authorize', data);
  return response.data;
};
export const useTidalLoginMutation = () => {
  return useMutation({
    mutationFn: login,
  });
};
