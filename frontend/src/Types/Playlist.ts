import type { PlaylistTracksDetailed } from './Tracks';

type PlaylistImage = {
  url: string;
  height: number | null;
  width: number | null;
};
type PlaylistTracks = {
  href: string;
  total: number;
};
export type PlaylistApi = {
  id: string;
  images: PlaylistImage[];
  name: string;
  public: boolean;
  tracks: PlaylistTracks;
};

export type TidalPlaylistId = {
  data: {
    id: string;
  };
};
export type Playlist = PlaylistApi & {
  picked: boolean;
  tracksDetailed: PlaylistTracksDetailed | null;
};
