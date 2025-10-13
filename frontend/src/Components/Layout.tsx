import type { ReactNode } from 'react';

type LayoutProps = {
  children: ReactNode;
};
export function Layout({ children }: LayoutProps) {
  return (
    <div className="flex min-h-screen flex-col">
      <main className="flex flex-1 bg-dark-blue px-4 py-10">
        <div className="mx-auto w-full max-w-screen-xl">{children}</div>
      </main>
    </div>
  );
}
