import { createFileRoute } from '@tanstack/react-router';
import { Button } from '../Components/Button';
import { ENV } from '../Api/utils';

export const Route = createFileRoute('/from')({
  component: RouteComponent,
});

function RouteComponent() {
  const clickToSpotify = () => {
    window.location.href = `${ENV.API_BASE_URL}api/Auth/spotify/login`;
  };
  return (
    <div className="flex h-full flex-col items-center justify-center">
      <div className="flex w-96 flex-col justify-center rounded-2xl bg-slate-600 p-4">
        <h1 className="text-center font-bold text-white"> Please log in to your Spotify account!</h1>
        <div className="mt-6 flex justify-center">
          <Button type="button" label="Spotify login" onClick={clickToSpotify} variant="primary" />
        </div>
      </div>
    </div>
  );
}
