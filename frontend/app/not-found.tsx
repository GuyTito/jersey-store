import Link from "next/link";

export default function NotFound() {
  return (
    <main className="mx-auto max-w-2xl p-8 text-center">
      <h1 className="text-3xl font-bold">Page not found</h1>
      <p className="mt-4 text-gray-600">
        The product you&apos;re looking for doesn&apos;t exist.
      </p>
      <Link
        href="/"
        className="mt-6 inline-block rounded bg-black px-6 py-3 font-semibold text-white"
      >
        Back to store
      </Link>
    </main>
  );
}