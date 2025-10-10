import type { PlaylistTrack } from '../Types/Tracks';
import { apiInstance } from './utils';
export async function fetchPlaylistTracks(state: string, playlistId: string): Promise<PlaylistTrack[]> {
  const response = await apiInstance.get(`api/Spotify/playlist/${playlistId}/tracks`, {
    params: { state },
  });
  return response.data;
}
