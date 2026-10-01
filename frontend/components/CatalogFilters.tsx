"use client";

import { useEffect, useRef, useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import {
  buildProductSearchHref,
  type ProductSearchFilters,
} from "@/lib/product-search";

type TextFilter = "q" | "club" | "season" | "minPrice" | "maxPrice";

const inputClassName = "filter-control";
const selectClassName = "filter-control";

export function CatalogFilters({
  initialFilters,
}: {
  initialFilters: ProductSearchFilters;
}) {
  const router = useRouter();
  const debounceTimer = useRef<ReturnType<typeof setTimeout> | null>(null);
  const [filters, setFilters] = useState(initialFilters);
  const [isPending, startTransition] = useTransition();

  useEffect(
    () => () => {
      if (debounceTimer.current) clearTimeout(debounceTimer.current);
    },
    [],
  );

  function navigate(nextFilters: ProductSearchFilters) {
    startTransition(() => {
      router.replace(
        buildProductSearchHref({ ...nextFilters, page: undefined }),
        {
          scroll: false,
        },
      );
    });
  }

  function updateTextFilter(name: TextFilter, value: string) {
    const nextFilters = {
      ...filters,
      [name]: value || undefined,
      page: undefined,
    };
    setFilters(nextFilters);

    if (debounceTimer.current) clearTimeout(debounceTimer.current);
    debounceTimer.current = setTimeout(() => navigate(nextFilters), 300);
  }

  function updateSelectFilter(name: "size" | "sort", value: string) {
    const nextFilters = {
      ...filters,
      [name]: value || undefined,
      page: undefined,
    };
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
    <form action="/" method="get" aria-busy={isPending} className="filter-form">
      <div className="filter-grid">
        <label className="filter-field">
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
        <label className="filter-field">
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
        <label className="filter-field">
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
        <label className="filter-field">
          Size in stock
          <select
            name="size"
            value={filters.size ?? ""}
            onChange={(event) => updateSelectFilter("size", event.target.value)}
            className={selectClassName}
          >
            <option value="">Any size</option>
            {["S", "M", "L", "XL"].map((size) => (
              <option key={size} value={size}>
                {size}
              </option>
            ))}
          </select>
        </label>
        <label className="filter-field">
          Minimum price
          <input
            type="number"
            name="minPrice"
            min="0"
            step="0.01"
            value={filters.minPrice ?? ""}
            onChange={(event) =>
              updateTextFilter("minPrice", event.target.value)
            }
            className={inputClassName}
          />
        </label>
        <label className="filter-field">
          Maximum price
          <input
            type="number"
            name="maxPrice"
            min="0"
            step="0.01"
            value={filters.maxPrice ?? ""}
            onChange={(event) =>
              updateTextFilter("maxPrice", event.target.value)
            }
            className={inputClassName}
          />
        </label>
        <label className="filter-field">
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
      <div className="filter-form__footer">
        <button type="button" onClick={resetFilters} className="filter-reset">
          Reset
        </button>
        {isPending && (
          <span role="status" className="filter-status">
            Updating results...
          </span>
        )}
      </div>
    </form>
  );
}
