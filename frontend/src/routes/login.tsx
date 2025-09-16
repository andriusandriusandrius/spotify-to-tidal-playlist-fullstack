import { createFileRoute } from '@tanstack/react-router';
import { Button } from '../Components/Button';

export const Route = createFileRoute('/login')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="flex h-full w-full items-center justify-center">
      <main className="flex flex-col gap-4">
        <Button type="button" label="Spotify" variant="primary" />
        <Button type="button" label="Tidal" variant="primary" />
        <Button type="button" label="Finish set up" variant="primary" disabled={true} />
      </main>
    </div>
  );
}
