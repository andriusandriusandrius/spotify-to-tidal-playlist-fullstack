import type { Playlist } from '../Types/Playlist';

type PlaylistProps = {
  src: Playlist | null;
};

export function PlaylistCard({ src }: PlaylistProps) {
  const imageUrl = src?.images?.[0]?.url ?? '/placeholder.png';
  return (
    <div className="flex items-center rounded-md bg-slate-600 p-2">
      <input type="checkbox" className="mr-3 ml-2 h-10 w-10 accent-blue-500" />
      <img className="h-12 w-12 rounded-md object-cover" src={imageUrl} />
      <div className="ml-2 flex w-full flex-col overflow-hidden">
        <h2 className="truncate font-bold text-white">{src?.name}</h2>

        <p className="text-sm text-gray-200">
          {src?.tracks.total} tracks | {src?.public ? 'Public' : 'Private'}
        </p>
      </div>
    </div>
  );
}
