import type { Playlist } from '../Types/Playlist';

type PlaylistProps = {
  src: Playlist | null;
};

export function PlaylistCard({ src }: PlaylistProps) {
  const imageUrl = src?.images?.[0]?.url ?? '/placeholder.png';
  return (
    <div className="flex">
      <img src={imageUrl} />
    </div>
  );
}
