import Link from "next/link";
import { StorefrontHeader } from "@/components/StorefrontHeader";

export default function NotFound() {
  return (
    <>
      <StorefrontHeader />
      <main className="error-page">
        <span className="section-eyebrow">404 · Jersey not found</span>
        <h1>Page not found</h1>
        <p>The product you&apos;re looking for doesn&apos;t exist.</p>
        <div className="home-hero__actions">
          <Link href="/" className="button button--lime">
            Back to store
          </Link>
        </div>
      </main>
    </>
  );
}
