export type PlaylistTrack = {
  track: {
    id: string;
    name: string;
    external_ids: {
      isrc: string;
    };
  };
};
export type TidalTrackId = {
  id: string;
};
