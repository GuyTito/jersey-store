"use client";

import { useEffect, useRef, useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { buildProductSearchHref, type ProductSearchFilters } from "@/lib/product-search";

type TextFilter = "q" | "club" | "season" | "minPrice" | "maxPrice";

const inputClassName = "mt-1 block w-full rounded border border-gray-300 px-3 py-2 font-normal";
const selectClassName = "mt-1 block w-full rounded border border-gray-300 bg-white px-3 py-2 font-normal";

export function CatalogFilters({ initialFilters }: { initialFilters: ProductSearchFilters }) {
  const router = useRouter();
  const debounceTimer = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [filters, setFilters] = useState(initialFilters);
  const [isPending, startTransition] = useTransition();

  useEffect(() => () => {
    if (debounceTimer.current) clearTimeout(debounceTimer.current);
  }, []);

  function navigate(nextFilters: ProductSearchFilters) {
    startTransition(() => {
      router.replace(buildProductSearchHref({ ...nextFilters, page: undefined }), {
        scroll: false,
      });
    });
  }

  function updateTextFilter(name: TextFilter, value: string) {
    const nextFilters = { ...filters, [name]: value || undefined, page: undefined };
    setFilters(nextFilters);

    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    debounceTimer.current = setTimeout(() => navigate(nextFilters), 300);
  }

  function updateSelectFilter(name: "size" | "sort", value: string) {
    const nextFilters = { ...filters, [name]: value || undefined, page: undefined };
    setFilters(nextFilters);

    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    navigate(nextFilters);
  }

  function resetFilters() {
    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    setFilters({ sort: "featured", page: 1 });
    startTransition(() => router.replace("/", { scroll: false }));
  }

  return (
    <form
      action="/"
      method="get"
      aria-busy={isPending}
      className="mb-7 border-y border-gray-200 py-5"
    >
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <label className="text-sm font-medium">
          Search
          <input
            type="search"
            name="q"
            maxLength={100}
            value={filters.q ?? ""}
            onChange={(event) => updateTextFilter("q", event.target.value)}
            placeholder="Club or jersey name"
            className={inputClassName}
          />
        </label>
        <label className="text-sm font-medium">
          Club
          <input
            type="text"
            name="club"
            value={filters.club ?? ""}
            onChange={(event) => updateTextFilter("club", event.target.value)}
            placeholder="Any club"
            className={inputClassName}
          />
        </label>
        <label className="text-sm font-medium">
          Season
          <input
            type="text"
            name="season"
            value={filters.season ?? ""}
            onChange={(event) => updateTextFilter("season", event.target.value)}
            placeholder="Any season"
            className={inputClassName}
          />
        </label>
        <label className="text-sm font-medium">
          Size in stock
          <select
            name="size"
            value={filters.size ?? ""}
            onChange={(event) => updateSelectFilter("size", event.target.value)}
            className={selectClassName}
          >
            <option value="">Any size</option>
            {["S", "M", "L", "XL"].map((size) => (
              <option key={size} value={size}>{size}</option>
            ))}
          </select>
        </label>
        <label className="text-sm font-medium">
          Minimum price
          <input
            type="number"
            name="minPrice"
            min="0"
            step="0.01"
            value={filters.minPrice ?? ""}
            onChange={(event) => updateTextFilter("minPrice", event.target.value)}
            className={inputClassName}
          />
        </label>
        <label className="text-sm font-medium">
          Maximum price
          <input
            type="number"
            name="maxPrice"
            min="0"
            step="0.01"
            value={filters.maxPrice ?? ""}
            onChange={(event) => updateTextFilter("maxPrice", event.target.value)}
            className={inputClassName}
          />
        </label>
        <label className="text-sm font-medium">
          Sort by
          <select
            name="sort"
            value={filters.sort ?? "featured"}
            onChange={(event) => updateSelectFilter("sort", event.target.value)}
            className={selectClassName}
          >
            <option value="featured">Featured</option>
            <option value="name-asc">Name, A to Z</option>
            <option value="price-asc">Price, low to high</option>
            <option value="price-desc">Price, high to low</option>
          </select>
        </label>
      </div>
      <div className="mt-4 flex min-h-6 items-center gap-4">
        <button
          type="button"
          onClick={resetFilters}
          className="text-sm font-medium underline underline-offset-4"
        >
          Reset
        </button>
        {isPending && <span role="status" className="text-sm text-gray-600">Updating results...</span>}
      </div>
    </form>
  );
}