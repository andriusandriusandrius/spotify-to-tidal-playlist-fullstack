import { createFileRoute } from '@tanstack/react-router';
import { useGetPlaylists } from '../../Api/useGetPlaylists';
import { PlaylistCard } from '../../Components/PlaylistCard';
import type { Playlist } from '../../Types/Playlist';
import { useEffect, useState } from 'react';
import { Button } from '../../Components/Button';
import { fetchPlaylistTracks } from '../../Api/fetchPlaylistTracks';

export const Route = createFileRoute('/transfer/success')({
  component: RouteComponent,
});

function RouteComponent() {
  const fromState = localStorage.getItem('fromState') ?? '';
  const { data, isLoading, isError } = useGetPlaylists(fromState);
  const [playlists, setPlaylists] = useState<Playlist[] | null>(null);

  useEffect(() => {
    if (data) {
      const playlists: Playlist[] = data.map((playlist) => ({ ...playlist, picked: false, tracksDetailed: null }));
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

  const handleSelectAll = () => {
    setPlaylists((prev) => {
      if (!prev) return prev;
      const allPicked = prev.every((p) => p.picked === true);
      return prev.map((pl) => ({ ...pl, picked: !allPicked }));
    });
  };
  const transferSongs = async () => {
    const pickedPlaylists = playlists?.filter((playlist) => {
      return playlist.picked === true;
    });
    if (!pickedPlaylists) return;
    pickedPlaylists.forEach(async (playlist) => {
      const id = playlist.id;
      console.log(id);
      console.log(fromState);
      const tracks = await fetchPlaylistTracks(fromState, id);
      playlist.tracksDetailed = tracks;
    });
  };
  if (isLoading) {
    return <></>;
  }
  if (isError) {
    return <></>;
  }

  return (
    <div className="flex flex-col items-center justify-center gap-4">
      <div className="flex gap-2">
        <Button type="button" label="Transfer playlists" variant="primary" onClick={transferSongs} />
        <Button type="button" label="Select All" variant="primary" onClick={handleSelectAll} />
      </div>
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
