import type { Playlist } from '../Types/Playlist';

type PlaylistProps = {
  src: Playlist | null;
};

export function PlaylistCard({ src }: PlaylistProps) {
  return (
    <div className="flex">
      <img src={src?.images[0].url} />
    </div>
  );
}
