import { createFileRoute, useSearch } from '@tanstack/react-router';

import type { RedirParam } from '../../Types/Auth';
import { useEffect } from 'react';
import { StreamingServiceTemplate } from '../../Components/StreamingServiceTemplate';

export const Route = createFileRoute('/to/success')({
  component: RouteComponent,
});

function RouteComponent() {
  const search: RedirParam = useSearch({ from: '/to/success' });

  useEffect(() => {
    if (search?.state) {
      localStorage.setItem('fromState', search.state);
    }
  }, [search]);

  return (
    <div className="flex h-full flex-col items-center justify-center gap-4">
      <h1 className="mb-4 text-5xl text-dark-brown">Select destination:</h1>
      <div className="flex w-full justify-center gap-4">
        <StreamingServiceTemplate service="tidal" />
        <StreamingServiceTemplate service="none" />
        <StreamingServiceTemplate service="none" />
      </div>
    </div>
  );
}
