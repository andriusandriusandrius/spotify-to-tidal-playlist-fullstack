type PlaylistImage = {
  url: string;
  height: number | null;
  width: number | null;
};
type PlaylistTrack = {
  href: string;
  total: number;
};

export type PlaylistApi = {
  id: string;
  images: PlaylistImage[];
  name: string;
  public: boolean;
  tracks: PlaylistTrack;
};
export type Playlist = PlaylistApi & {
  picked: boolean;
};
