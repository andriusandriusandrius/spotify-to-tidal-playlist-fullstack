import { createFileRoute } from '@tanstack/react-router';
import { StreamingServiceTemplate } from '../Components/StreamingServiceTemplate';

export const Route = createFileRoute('/from')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="flex h-full flex-col items-center justify-center gap-4">
      <h1 className="mb-4 text-5xl text-dark-brown">Select source:</h1>
      <div className="flex w-full justify-center gap-4">
        <StreamingServiceTemplate service="spotify" />
        <StreamingServiceTemplate service="none" />
        <StreamingServiceTemplate service="none" />
      </div>
    </div>
  );
}
