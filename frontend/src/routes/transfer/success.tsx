import { createFileRoute, useSearch } from '@tanstack/react-router';
import { useGetPlaylists } from '../../Api/useGetPlaylists';
import { PlaylistCard } from '../../Components/PlaylistCard';
import type { Playlist } from '../../Types/Playlist';
import { useEffect, useState } from 'react';
import { Button } from '../../Components/Button';
import { fetchPlaylistTracks } from '../../Api/fetchPlaylistTracks';
import type { RedirParam } from '../../Types/Auth';
import { fetchTidalSearchTrack } from '../../Api/fetchTidalSearchTrack';
import { createTidalPlaylist } from '../../Api/createTidalPlaylist';
import type { PlaylistTrack } from '../../Types/Tracks';
import { populatePlaylist } from '../../Api/populatePlaylist';

export const Route = createFileRoute('/transfer/success')({
  component: RouteComponent,
});

async function findTidalTracks(playlistItems: PlaylistTrack[], tidalTrackIds: string[], toState: string) {
  const delay = (ms: number) => new Promise((res) => setTimeout(res, ms));
  let foundCount = 0;
  let skippedCount = 0;
  for (const element of playlistItems) {
    const isrc = element.external_ids.isrc.toUpperCase();

    if (!isrc) {
      console.warn('Skipping track without ISRC:', element.name);
      skippedCount++;
      continue;
    }
    try {
      const foundTrackId = await fetchTidalSearchTrack(toState, isrc);
      if (!foundTrackId || !foundTrackId.id) {
        console.warn('Track not on tidal', element.name);
        skippedCount++;
        continue;
      }

      tidalTrackIds.push(foundTrackId.id);
      foundCount++;
      await delay(300);
    } catch (error) {
      console.error('Error searching for track:', element.name, error);
      skippedCount++;
    }
  }
  console.log(`Found ${foundCount} tracks, skipped ${skippedCount} tracks`);
}

function RouteComponent() {
  const fromState = localStorage.getItem('fromState') ?? '';
  const toStateQuery: RedirParam = useSearch({ from: '/transfer/success' });
  const { data, isLoading, isError } = useGetPlaylists(fromState);
  const [playlists, setPlaylists] = useState<Playlist[] | null>(null);

  const toState = toStateQuery.state;

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

    if (!pickedPlaylists || pickedPlaylists.length === 0) {
      console.warn('No playlists selected');
      return;
    }

    console.log(`Starting transfer of ${pickedPlaylists.length} playlists`);

    for (const playlist of pickedPlaylists) {
      const spotifyPlaylistId = playlist.id;

      if (!spotifyPlaylistId) {
        console.error('Playlist missing ID:', playlist.name);
        continue;
      }
      console.log(`Processing playlist: ${playlist.name}`);

      const tidalTrackIds: string[] = [];

      try {
        const tracks = await fetchPlaylistTracks(fromState, spotifyPlaylistId);
        console.log(`Found ${tracks.length} tracks in playlist`);

        const tidalPlaylistId = await createTidalPlaylist(playlist, toState);
        if (!tidalPlaylistId) {
          console.error('Failed to create Tidal playlist for:', playlist.name);
          continue;
        }
        console.log(`Created Tidal playlist with ID: ${tidalPlaylistId.data.id}`);

        await findTidalTracks(tracks, tidalTrackIds, toState);

        if (tidalTrackIds.length === 0) {
          console.warn('No tracks found on Tidal for playlist:', playlist.name);
          continue;
        }

        console.log(`Populating playlist with ${tidalTrackIds.length} tracks`);

        await populatePlaylist(tidalPlaylistId, tidalTrackIds, toState);

        console.log(`Successfully transferred playlist: ${playlist.name}`);
      } catch (error) {
        console.error(`Error transferring playlist ${playlist.name}:`, error);
      }
    }
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
