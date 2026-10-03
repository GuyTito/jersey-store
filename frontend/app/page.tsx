import type { Metadata } from "next";
import Link from "next/link";
import { CatalogFilters } from "@/components/CatalogFilters";
import { ProductImage } from "@/components/ProductImage";
import { StorefrontHeader } from "@/components/StorefrontHeader";
import { WhatsAppButton } from "@/components/WhatsAppButton";
import { getProducts } from "@/lib/api";
import {
  buildProductSearchHref,
  type ProductSearchFilters,
} from "@/lib/product-search";
import { formatPrice } from "@/lib/format";

type PageSearchParams = Record<string, string | string[] | undefined>;

const firstValue = (value: string | string[] | undefined) =>
  Array.isArray(value) ? value[0] : value;

function readFilters(searchParams: PageSearchParams): ProductSearchFilters {
  const sort = firstValue(searchParams.sort);
  const rawPage = Number(firstValue(searchParams.page));

  return {
    q: firstValue(searchParams.q)?.trim() || undefined,
    club: firstValue(searchParams.club)?.trim() || undefined,
    season: firstValue(searchParams.season)?.trim() || undefined,
    size: firstValue(searchParams.size)?.trim() || undefined,
    minPrice: firstValue(searchParams.minPrice)?.trim() || undefined,
    maxPrice: firstValue(searchParams.maxPrice)?.trim() || undefined,
    sort:
      sort === "name-asc" || sort === "price-asc" || sort === "price-desc"
        ? sort
        : "featured",
    page: Number.isSafeInteger(rawPage) && rawPage > 0 ? rawPage : 1,
  };
}

function getPriceError(filters: ProductSearchFilters) {
  const minPrice =
    filters.minPrice === undefined ? undefined : Number(filters.minPrice);
  const maxPrice =
    filters.maxPrice === undefined ? undefined : Number(filters.maxPrice);

  if (
    (minPrice !== undefined && (!Number.isFinite(minPrice) || minPrice < 0)) ||
    (maxPrice !== undefined && (!Number.isFinite(maxPrice) || maxPrice < 0))
  ) {
    return "Enter valid, non-negative price limits.";
  }

  if (minPrice !== undefined && maxPrice !== undefined && minPrice > maxPrice) {
    return "Minimum price must not exceed maximum price.";
  }

  return null;
}

export async function generateMetadata({
  searchParams,
}: {
  searchParams: Promise<PageSearchParams>;
}): Promise<Metadata> {
  const { q, club } = readFilters(await searchParams);
  if (q) return { title: `Search: ${q}` };
  if (club) return { title: `${club} jerseys` };
  return { title: { absolute: "Jersey Store | Authentic club jerseys" } };
}

export default async function Home({
  searchParams,
}: {
  searchParams: Promise<PageSearchParams>;
}) {
  const filters = readFilters(await searchParams);
  const priceError = getPriceError(filters);
  const catalog = priceError ? null : await getProducts(filters);

  return (
    <div className="storefront-page">
      <StorefrontHeader />
      <WhatsAppButton />
      <main>
        <section className="home-hero">
          <div className="site-container home-hero__inner">
            <div className="home-hero__copy">
              <span className="eyebrow">Club jerseys · 2025/26</span>
              <h1>
                Wear the colors
                <br />
                <span>of your club.</span>
              </h1>
              <p>
                Find your team, choose a size in stock, and order your jersey
                directly on WhatsApp.
              </p>
              <div className="home-hero__actions">
                <Link href="#catalog" className="button button--lime">
                  Shop all jerseys <span aria-hidden="true">→</span>
                </Link>
              </div>
            </div>
          </div>
        </section>

        <section className="site-container catalog-section" id="catalog">
          <div className="section-heading">
            <div>
              <span className="section-eyebrow">The collection</span>
              <h2>Shop jerseys</h2>
            </div>
          </div>

          <CatalogFilters initialFilters={filters} />

          {priceError ? (
            <p role="alert" className="catalog-error">
              {priceError}
            </p>
          ) : catalog ? (
            <>
              <div className="section-heading">
                <p className="catalog-count">
                  {catalog.totalCount}{" "}
                  {catalog.totalCount === 1 ? "jersey" : "jerseys"}
                </p>
                {catalog.totalCount > 0 && (
                  <p className="catalog-page-count">
                    Page {catalog.page} of{" "}
                    {Math.ceil(catalog.totalCount / catalog.pageSize)}
                  </p>
                )}
              </div>

              {catalog.items.length > 0 ? (
                <ul className="product-grid">
                  {catalog.items.map((product) => (
                    <li key={product.id} className="product-card">
                      <Link
                        href={`/products/${product.slug}`}
                        className="product-card__link"
                      >
                        <div className="product-card__image-wrap">
                          <ProductImage
                            src={product.imageUrl}
                            alt={product.name}
                          />
                        </div>
                        <h3 className="product-card__name">{product.name}</h3>
                        <p className="product-card__meta">
                          {product.club} · {product.season} · {product.kitType}
                        </p>
                        <p
                          className={`product-card__price${product.lowestInStockPrice === null ? " product-card__price--sold-out" : ""}`}
                        >
                          {product.lowestInStockPrice === null
                            ? "Sold out"
                            : formatPrice(product.lowestInStockPrice)}
                        </p>
                      </Link>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="catalog-empty">No jerseys match these filters.</p>
              )}

              {catalog.totalCount > catalog.pageSize && (
                <nav aria-label="Catalog pages" className="catalog-pagination">
                  {catalog.page > 1 ? (
                    <Link
                      href={buildProductSearchHref({
                        ...filters,
                        page: catalog.page - 1,
                      })}
                      rel="prev"
                    >
                      ← Previous
                    </Link>
                  ) : (
                    <span className="catalog-pagination__spacer" />
                  )}
                  {catalog.page * catalog.pageSize < catalog.totalCount && (
                    <Link
                      href={buildProductSearchHref({
                        ...filters,
                        page: catalog.page + 1,
                      })}
                      rel="next"
                    >
                      Next →
                    </Link>
                  )}
                </nav>
              )}
            </>
          ) : null}
        </section>
      </main>
    </div>
  );
}
