export default function Loading() {
  return (
    <main className="mx-auto max-w-6xl px-6 py-10">
      <header className="mb-8">
        <div className="h-8 w-48 animate-pulse bg-gray-200 rounded" />
        <div className="mt-2 h-4 w-64 animate-pulse bg-gray-200 rounded" />
      </header>
      <ul className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        {Array.from({ length: 8 }).map((_, i) => (
          <li key={i}>
            <div className="aspect-[4/5] w-full rounded-lg animate-pulse bg-gray-200" />
            <div className="mt-3 h-5 w-3/4 animate-pulse bg-gray-200 rounded" />
            <div className="mt-1 h-4 w-1/2 animate-pulse bg-gray-200 rounded" />
            <div className="mt-1 h-5 w-1/4 animate-pulse bg-gray-200 rounded" />
          </li>
        ))}
      </ul>
    </main>
  );
}