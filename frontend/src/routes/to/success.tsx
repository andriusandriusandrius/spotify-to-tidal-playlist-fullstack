import { createFileRoute, useSearch } from '@tanstack/react-router';
import { Button } from '../../Components/Button';
import { ENV } from '../../Api/utils';
import type { RedirParam } from '../../Types/Auth';
import { useEffect } from 'react';

export const Route = createFileRoute('/to/success')({
  component: RouteComponent,
});

function RouteComponent() {
  const search: RedirParam = useSearch({ from: '/to/success' });
  const clickToTidal = () => {
    window.location.href = `${ENV.API_BASE_URL}api/Auth/tidal/login`;
  };
  useEffect(() => {
    if (search?.state) {
      localStorage.setItem('fromState', search.state);
    }
  }, [search]);
  return (
    <div className="flex h-full flex-col items-center justify-center">
      <div className="flex w-96 flex-col justify-center rounded-2xl bg-slate-600 p-4">
        <h1 className="text-center font-bold text-white"> Please log in to your Tidal account!</h1>
        <div className="mt-6 flex justify-center">
          <Button type="button" label="Spotify login" onClick={clickToTidal} variant="primary" />
        </div>
      </div>
    </div>
  );
}
