import type { TidalPlaylistId } from '../Types/Playlist';
import { apiInstance } from './utils';

export async function populatePlaylist(tidalPlaylistId: TidalPlaylistId, tidalTrackIds: string[], state: string) {
  const id = tidalPlaylistId.data.id;

  console.log(`Adding ${tidalTrackIds.length} tracks in batches of 20...`);
  const chunkSize = 20;
  const chunks = [];
  const results = [];
  for (let i = 0; i < tidalTrackIds.length; i += chunkSize) {
    chunks.push(tidalTrackIds.slice(i, i + chunkSize));
  }

  for (let i = 0; i < chunks.length; i++) {
    const chunk = chunks[i];
    console.log(`Sending batch ${i + 1}/${chunks.length} (${chunk.length} tracks)`);
    try {
      const response = await apiInstance.post(`/api/tidal/playlists/${id}/track`, chunk, {
        params: { state },
      });

      results.push(response.data);
      console.log(`Successfully added batch ${i + 1}/${chunks.length}`);

      if (i < chunks.length - 1) {
        await new Promise((resolve) => setTimeout(resolve, 500));
      }
    } catch (error) {
      console.error(`Error adding batch ${i + 1}:`, error);
      throw error;
    }
  }
  return results;
}
