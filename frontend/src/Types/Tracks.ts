type PlaylistTrack = {
  id: string;
  name: string;
  external_ids: {
    isrc: string;
  };
};
export type PlaylistTracksDetailed = {
  href: string;
  items: PlaylistTrack[];
};
