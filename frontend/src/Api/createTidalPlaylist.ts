import type { Playlist, TidalPlaylistId } from '../Types/Playlist';
import { apiInstance } from './utils';

export async function createTidalPlaylist(playlist: Playlist, state: string): Promise<TidalPlaylistId> {
  const response = await apiInstance.post('api/tidal/playlists', null, {
    params: {
      accessType: playlist.public ? 'Public' : 'Private',
      description:
        playlist.description && playlist.description.trim().length > 0
          ? playlist.description
          : 'imported from spotify via transfify',
      name: playlist.name,
      state: state,
    },
  });

  return response.data;
}
