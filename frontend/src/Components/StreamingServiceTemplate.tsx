import { ENV } from '../Api/utils';

type Props = {
  service: 'spotify' | 'tidal' | 'none';
};

export function StreamingServiceTemplate({ service }: Props) {
  function handleClick() {
    window.location.href = `${ENV.API_BASE_URL}api/Auth/${service}/login`;
  }
  return (
    <>
      {service === 'none' && (
        <div className="h-32 w-full max-w-52 min-w-16 cursor-not-allowed rounded-md bg-gradient-to-b from-light-blue to-middle-blue p-4 opacity-50 hover:opacity-100"></div>
      )}
      {service === 'spotify' && (
        <div
          className="flex h-32 w-full max-w-52 min-w-16 cursor-pointer items-center justify-center rounded-md bg-spotify-green p-4 opacity-50 hover:opacity-100"
          onClick={handleClick}
        >
          <img src="/spotifyLogo.png" alt="Spotify" />
        </div>
      )}
      {service === 'tidal' && (
        <div
          className="flex h-32 w-full max-w-52 min-w-16 cursor-pointer items-center justify-center rounded-md bg-white p-4 opacity-50 hover:opacity-100"
          onClick={handleClick}
        >
          <img src="/tidalLogo.svg" alt="Tidal" />
        </div>
      )}
    </>
  );
}
