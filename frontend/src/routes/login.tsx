import { createFileRoute } from '@tanstack/react-router';
import { Button } from '../Components/Button';
import { useTidalLoginMutation } from '../Api/useTidalLoginMutation';
import type { TidalLoginRequest } from '../Api/useTidalLoginMutation';
import { ENV } from '../Api/env';
import { codeVerifier, generateCodeChallange, generateState } from '../Api/pkceUtil';
export const Route = createFileRoute('/login')({
  component: RouteComponent,
});

function RouteComponent() {
  const { mutate } = useTidalLoginMutation();
  const onTidalClick = () => {
    const scope = ['playlists.write', 'playlits.read'];
    const verifier = codeVerifier();
    const code_challange = generateCodeChallange(verifier);
    const state = generateState();

    const data: TidalLoginRequest = {
      client_id: ENV.TIDAL_CLIENT_ID,
      redirect_uri: ENV.FRONTEND_REDIR,
      scope: scope,
      code_challange_method: 'S256',
      code_challange: code_challange,
      state: state,
    };
    mutate(data);
  };
  return (
    <div className="flex h-full w-full items-center justify-center">
      <main className="flex flex-col gap-4">
        <Button type="button" label="Spotify" variant="primary" />
        <Button type="button" label="Tidal" variant="primary" onClick={onTidalClick} />
        <Button type="button" label="Finish set up" variant="primary" disabled={true} />
      </main>
    </div>
  );
}
