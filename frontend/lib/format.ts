export function formatPrice(amount: number, currencyCode = "GHS"): string {
  return new Intl.NumberFormat("en-GH", {
    style: "currency",
    currency: currencyCode,
    minimumFractionDigits: 2,
  }).format(amount);
}