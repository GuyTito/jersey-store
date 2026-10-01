export type ProductSearchFilters = {
  q?: string;
  club?: string;
  season?: string;
  size?: string;
  minPrice?: string;
  maxPrice?: string;
  sort?: "featured" | "name-asc" | "price-asc" | "price-desc";
  page?: number;
};

export type PagedResponse<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
};

export function serializeProductSearch(filters: ProductSearchFilters) {
  const params = new URLSearchParams();

  for (const [key, value] of Object.entries(filters)) {
    if (value !== undefined && value !== "") {
      params.set(key, String(value));
    }
  }

  return params.toString();
}

export const buildProductSearchHref = (filters: ProductSearchFilters) => {
  const query = serializeProductSearch(filters);
  return query ? `/?${query}` : "/";
};
