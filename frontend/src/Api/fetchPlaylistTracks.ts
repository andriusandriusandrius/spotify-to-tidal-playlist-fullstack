import type { PlaylistTrack } from '../Types/Tracks';
import { apiInstance } from './utils';
export async function fetchPlaylistTracksDetailed(state: string, playlistId: string): Promise<PlaylistTrack[]> {
  const response = await apiInstance.get(`api/Spotify/playlist/${playlistId}`, {
    params: { state },
  });
  return response.data.tracks.items;
}
