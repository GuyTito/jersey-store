import "server-only";
import {
  serializeProductSearch,
  type PagedResponse,
  type ProductSearchFilters,
} from "./product-search";

const API_URL = process.env.API_URL;

if (!API_URL) {
  throw new Error("API_URL is not configured.");
}

async function request<T>(path: string): Promise<T> {
  const response = await fetch(`${API_URL}${path}`, { cache: "no-store" });

  if (response.status === 404) return null as T;

  if (!response.ok) {
    throw new Error(`API ${path} responded with HTTP ${response.status}.`);
  }

  return (await response.json()) as T;
}

export type ProductVariant = {
  id: number;
  size: string;
  price: number;
  stock: number;
};

export type ProductSummary = {
  id: number;
  slug: string;
  name: string;
  club: string;
  season: string;
  kitType: string;
  imageUrl: string;
  lowestInStockPrice: number | null;
};

export type ProductDetail = ProductSummary & {
  description: string;
  variants: ProductVariant[];
};

export type Storefront = { whatsAppNumber: string; currencyCode: string };

export const getProducts = (filters: ProductSearchFilters = {}) => {
  const query = serializeProductSearch(filters);
  return request<PagedResponse<ProductSummary>>(
    `/api/products${query ? `?${query}` : ""}`,
  );
};

export const getProductBySlug = (slug: string) =>
  request<ProductDetail | null>(`/api/products/${encodeURIComponent(slug)}`);
export const getStorefront = () => request<Storefront>("/api/storefront");
