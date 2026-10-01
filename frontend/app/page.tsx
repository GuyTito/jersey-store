import Image from "next/image";
import Link from "next/link";
import { CatalogFilters } from "@/components/CatalogFilters";
import { getProducts } from "@/lib/api";
import { buildProductSearchHref, type ProductSearchFilters } from "@/lib/product-search";
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
  const minPrice = filters.minPrice === undefined ? undefined : Number(filters.minPrice);
  const maxPrice = filters.maxPrice === undefined ? undefined : Number(filters.maxPrice);

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

export default async function Home({
  searchParams,
}: {
  searchParams: Promise<PageSearchParams>;
}) {
  const filters = readFilters(await searchParams);
  const priceError = getPriceError(filters);
  const catalog = priceError ? null : await getProducts(filters);

  return (
    <main className="mx-auto max-w-6xl px-6 py-10">
      <header className="mb-8">
        <h1 className="text-3xl font-bold">Jersey Store</h1>
        <p className="mt-1 text-gray-600">Authentic club jerseys, ordered over WhatsApp.</p>
      </header>

      <CatalogFilters initialFilters={filters} />

      {priceError ? (
        <p role="alert" className="py-8 text-sm font-medium text-red-700">{priceError}</p>
      ) : catalog ? (
        <>
          <div className="mb-4 flex flex-wrap items-baseline justify-between gap-2">
            <p className="text-sm text-gray-600">
              {catalog.totalCount} {catalog.totalCount === 1 ? "jersey" : "jerseys"}
            </p>
            {catalog.totalCount > 0 && (
              <p className="text-sm text-gray-600">
                Page {catalog.page} of {Math.ceil(catalog.totalCount / catalog.pageSize)}
              </p>
            )}
          </div>

          {catalog.items.length > 0 ? (
            <ul className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
              {catalog.items.map((product) => (
                <li key={product.id}>
                  <Link href={`/products/${product.slug}`} className="group block">
                    <Image
                      src={product.imageUrl}
                      alt={product.name}
                      width={800}
                      height={1000}
                      className="aspect-4/5 w-full rounded-lg object-cover"
                    />
                    <h2 className="mt-3 font-semibold group-hover:underline">{product.name}</h2>
                    <p className="text-sm text-gray-600">
                      {product.club} · {product.season} · {product.kitType}
                    </p>
                    <p className="mt-1 font-medium">
                      {product.lowestInStockPrice === null
                        ? "Sold out"
                        : formatPrice(product.lowestInStockPrice)}
                    </p>
                  </Link>
                </li>
              ))}
            </ul>
          ) : (
            <p className="border-y border-gray-200 py-12 text-center text-gray-600">
              No jerseys match these filters.
            </p>
          )}

          {catalog.totalCount > catalog.pageSize && (
            <nav aria-label="Catalog pages" className="mt-8 flex items-center justify-between border-t border-gray-200 pt-5">
              {catalog.page > 1 ? (
                <Link
                  href={buildProductSearchHref({ ...filters, page: catalog.page - 1 })}
                  rel="prev"
                  className="font-medium underline underline-offset-4"
                >
                  Previous
                </Link>
              ) : <span />}
              {catalog.page * catalog.pageSize < catalog.totalCount && (
                <Link
                  href={buildProductSearchHref({ ...filters, page: catalog.page + 1 })}
                  rel="next"
                  className="font-medium underline underline-offset-4"
                >
                  Next
                </Link>
              )}
            </nav>
          )}
        </>
      ) : null}
    </main>
  );
}