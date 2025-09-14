import type { ReactNode } from 'react';

type LayoutProps = {
  children: ReactNode;
};
export function Layout({ children }: LayoutProps) {
  return (
    <div className="flex min-h-screen flex-col">
      <header className="relative z-50 bg-amber-600 p-4 text-white shadow-md"></header>
      <main className="flex flex-1 px-4 py-10">
        <div className="mx-auto w-full max-w-screen-xl">{children}</div>
      </main>
      <footer className="bg-amber-600 p-4 text-center text-white">
        <div className="mx-auto w-full max-w-screen-xl">
          <p className="text-sm">{`${new Date().getFullYear()} Transfify.`}</p>
        </div>
      </footer>
    </div>
  );
}
