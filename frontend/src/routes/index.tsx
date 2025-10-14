import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { Button } from '../Components/Button';

export const Route = createFileRoute('/')({
  component: Index,
});

function Index() {
  const navigate = useNavigate();
  function handleClick() {
    navigate({ to: '/from' });
  }
  return (
    <main className="flex h-full flex-col items-center justify-center gap-8 text-dark-brown">
      <h1 className="mt-16 text-5xl">
        Transferify<p className="text-xl">A playlist transfer tool</p>
      </h1>
      <Button type="button" label="Click to proceed" variant="primary" onClick={handleClick} />
      <div className="flex w-full flex-col gap-4 rounded-md bg-light-blue p-4 text-center">
        <h3 className="font-bold">More about Transferify:</h3>
        <p>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit. In sit amet suscipit magna. Nullam nec luctus ipsum.
          Phasellus sed congue magna. Fusce sit amet vestibulum libero. Class aptent taciti sociosqu ad litora torquent
          per conubia nostra, per inceptos himenaeos. Etiam at lacinia augue. Nullam in quam tincidunt, luctus purus
          vitae, tincidunt ligula. Fusce placerat mi et libero aliquet, vel auctor ex pharetra. Donec ipsum tellus,
          gravida a commodo a, vulputate at felis. Ut pharetra tristique lectus, sit amet molestie quam rutrum a.
          Curabitur justo elit, euismod eu nulla quis, pharetra dignissim augue. Integer lectus neque, finibus non augue
          vel, pulvinar lobortis nibh. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia
          curae;
        </p>
      </div>
    </main>
  );
}
