import { useQuery } from '@tanstack/react-query';
import { apiInstance } from './utils';
import type { Playlist } from '../Types/Playlist';

async function fetchPlaylists(state: string): Promise<Playlist[]> {
  const response = await apiInstance.get('api/Spotify/playlist', {
    params: { state },
  });
  return response.data;
}

export const useGetPlaylists = (state: string) => {
  return useQuery({
    queryKey: ['playlists', state],
    queryFn: () => fetchPlaylists(state),
    enabled: !!state,
  });
};
