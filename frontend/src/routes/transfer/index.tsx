import { createFileRoute } from '@tanstack/react-router';
import { useGetPlaylists } from '../../Api/useGetPlaylists';
import { PlaylistCard } from '../../Components/PlaylistCard';
import type { Playlist } from '../../Types/Playlist';
import { useEffect, useState } from 'react';

export const Route = createFileRoute('/transfer/')({
  component: RouteComponent,
});

function RouteComponent() {
  const state = localStorage.getItem('state') ?? '';
  const { data, isLoading, isError } = useGetPlaylists(state);
  const [playlists, setPlaylists] = useState<Playlist[] | null>(null);

  useEffect(() => {
    if (data) setPlaylists(data);
    else setPlaylists(null);
  }, [data]);

  if (isLoading) {
    return <></>;
  }
  if (isError) {
    return <></>;
  }

  return (
    <div className="flex justify-center">
      <div className="h-full w-96 rounded-2xl bg-slate-400 p-8">
        {playlists?.map((playlist) => (
          <li key={playlist.id}>
            <PlaylistCard src={playlist} />
          </li>
        ))}
      </div>
    </div>
  );
}
