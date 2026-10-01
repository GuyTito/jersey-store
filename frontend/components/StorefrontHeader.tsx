import Link from "next/link";

export function StorefrontHeader() {
  return (
    <header className="store-header">
      <div className="site-container store-header__inner">
        <Link href="/" className="store-brand" aria-label="Jersey Store home">
          <span className="store-brand__mark" aria-hidden="true">
            J
          </span>
          <span>
            <span className="store-brand__wordmark">
              JERSEY<span>STORE</span>
            </span>
            <span className="store-brand__descriptor">
              Club kits, all season
            </span>
          </span>
        </Link>
        <nav className="store-nav" aria-label="Main navigation">
          <Link href="/">Home</Link>
          <Link href="/#catalog">Shop all</Link>
        </nav>
      </div>
    </header>
  );
}
