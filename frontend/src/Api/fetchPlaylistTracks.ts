import { apiInstance } from './utils';
import type { PlaylistTracksDetailed } from '../Types/Tracks';

export async function fetchPlaylistTracks(state: string, playlistId: string): Promise<PlaylistTracksDetailed> {
  const response = await apiInstance.get(`api/Spotify/playlist/${playlistId}`, {
    params: { state },
  });
  return response.data;
}
