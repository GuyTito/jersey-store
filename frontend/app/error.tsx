"use client";

import Link from "next/link";
import { StorefrontHeader } from "@/components/StorefrontHeader";

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <>
      <StorefrontHeader />
      <main className="error-page">
        <span className="section-eyebrow">Catalog unavailable</span>
        <h1>Something went wrong</h1>
        <p>{error.message}</p>
        <div className="home-hero__actions">
          <button onClick={reset} className="button button--lime">
            Try again
          </button>
          <Link href="/" className="button button--outline">
            Back to store
          </Link>
        </div>
      </main>
    </>
  );
}
