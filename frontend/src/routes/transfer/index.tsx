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
    if (data) {
      const playlists: Playlist[] = data.map((playlist) => ({ ...playlist, picked: false }));
      setPlaylists(playlists);
    } else {
      setPlaylists(null);
    }
  }, [data]);

  const handleTogglePicked = (id: string | undefined, checked: boolean) => {
    if (id) {
      setPlaylists((prev) =>
        prev ? prev.map((playlist) => (playlist.id === id ? { ...playlist, picked: checked } : playlist)) : prev,
      );
    }
  };

  if (isLoading) {
    return <></>;
  }
  if (isError) {
    return <></>;
  }

  return (
    <div className="flex justify-center">
      <div className="flex h-full w-96 flex-col gap-4 rounded-2xl bg-slate-400 p-8">
        {playlists?.map((playlist) => (
          <div key={playlist.id}>
            <PlaylistCard src={playlist} onToggle={handleTogglePicked} />
          </div>
        ))}
      </div>
    </div>
  );
}
