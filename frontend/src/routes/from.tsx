import { createFileRoute } from '@tanstack/react-router';
import { Button } from '../Components/Button';

export const Route = createFileRoute('/from')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className="flex h-full flex-col items-center justify-center">
      <div className="flex w-96 flex-col justify-center rounded-2xl bg-slate-600 p-4">
        <h1 className="text-center font-bold text-white"> Please log in to your Spotify account!</h1>
        <div className="mt-6 flex justify-center">
          <Button type="button" label="Spotify login" variant="primary" />
        </div>
      </div>
    </div>
  );
}
