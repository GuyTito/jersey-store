"use client";

import Link from "next/link";

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <main className="mx-auto max-w-2xl p-8 text-center">
      <h1 className="text-3xl font-bold">Something went wrong</h1>
      <p className="mt-4 text-gray-600">{error.message}</p>
      <div className="mt-6 flex gap-4 justify-center">
        <button
          onClick={reset}
          className="rounded bg-black px-6 py-3 font-semibold text-white"
        >
          Try again
        </button>
        <Link
          href="/"
          className="rounded border px-6 py-3 font-semibold"
        >
          Back to home
        </Link>
      </div>
    </main>
  );
}