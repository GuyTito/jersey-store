import { StorefrontHeader } from "@/components/StorefrontHeader";

export default function Loading() {
  return (
    <>
      <StorefrontHeader />
      <main className="site-container loading-page" aria-busy="true">
        <div className="loading-heading" />
        <ul className="loading-grid">
          {Array.from({ length: 8 }, (_, index) => (
            <li key={index}>
              <div className="loading-image" />
              <div className="loading-line" />
              <div className="loading-line loading-line--short" />
            </li>
          ))}
        </ul>
      </main>
    </>
  );
}
