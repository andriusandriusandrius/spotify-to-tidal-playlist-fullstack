import type { Playlist, TidalPlaylistId } from '../Types/Playlist';
import { apiInstance } from './utils';

export async function createTidalPlaylist(playlist: Playlist, state: string): Promise<TidalPlaylistId> {
  const response = await apiInstance.post('api/tidal/playlists', null, {
    params: {
      accessType: playlist.public ? 'Public' : 'Private',
      description: 'template',
      name: playlist.name,
      state: state,
    },
  });

  return response.data;
}
