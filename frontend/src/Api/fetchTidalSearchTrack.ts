import type { TidalTrackId } from '../Types/Tracks';
import { apiInstance } from './utils';

export async function fetchTidalSearchTrack(state: string, isrc: string): Promise<TidalTrackId> {
  const response = await apiInstance.get(`api/tidal/track/${isrc}`, {
    params: { state },
  });
  return await response.data[0];
}
