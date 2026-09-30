import { formatPrice } from "./format";

export type WhatsAppProduct = {
  name: string;
  slug: string;
  size: string;
  price: number;
  currencyCode: string;
};

export function buildWhatsAppUrl(
  phone: string,
  product: WhatsAppProduct,
): string {
  const message =
    `Hi, I'd like to buy: ${product.name} (Size: ${product.size}) - ` +
    `${formatPrice(product.price, product.currencyCode)}. Is it available? ` +
    `(ref: /products/${product.slug})`;

  return `https://wa.me/${phone.replace(/[^\d]/g, "")}?text=${encodeURIComponent(message)}`;
}
