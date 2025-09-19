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

const login = async (data: TidalLoginRequest) => {
  const response = await tidalInstance.post('/authorize', data);
  return response.data;
};
export const useTidalLoginMutation = () => {
  return useMutation({
    mutationFn: login,
  });
};
