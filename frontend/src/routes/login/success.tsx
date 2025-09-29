import { createFileRoute, useNavigate, useSearch } from '@tanstack/react-router';
import type { RedirParam } from '../../Types/Auth';
import { useEffect } from 'react';

export const Route = createFileRoute('/login/success')({
  component: RouteComponent,
});

function RouteComponent() {
  const navigate = useNavigate();
  const search: RedirParam = useSearch({ from: '/login/success' });
  useEffect(() => {
    if (search?.state) {
      localStorage.setItem('state', search.state);
      navigate({ to: '/transfer' });
    } else {
      navigate({ to: '/from' });
    }
  }, [search, navigate]);
  return <div></div>;
}
