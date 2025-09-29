type PlaylistImage = {
  url: string;
  height: number | null;
  width: number | null;
};
type PlaylistTrack = {
  href: string;
  total: number;
};

export type Playlist = {
  id: string;
  images: PlaylistImage[];
  name: string;
  public: boolean;
  tracks: PlaylistTrack;
};
