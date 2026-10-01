"use client";

import { useState } from "react";
import type { ProductDetail } from "@/lib/api";
import { formatPrice } from "@/lib/format";
import { buildWhatsAppUrl } from "@/lib/whatsapp";

type Props = {
  product: ProductDetail;
  whatsappNumber: string;
  currencyCode: string;
};

export default function ProductPurchasePanel({
  product,
  whatsappNumber,
  currencyCode,
}: Props) {
  const available = product.variants.filter((v) => v.stock > 0);
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const selected = product.variants.find((v) => v.id === selectedId) ?? null;

  if (available.length === 0) {
    return (
      <p className="mt-8 font-medium text-red-700">This product is sold out.</p>
    );
  }

  return (
    <section className="mt-8">
      <h2 className="font-semibold">Choose a size</h2>
      <div className="mt-3 flex flex-wrap gap-2">
        {product.variants.map((variant) => {
          const soldOut = variant.stock === 0;
          const isSelected = variant.id === selectedId;
          const classes = soldOut
            ? "cursor-not-allowed rounded border px-4 py-2 text-gray-400 line-through"
            : isSelected
              ? "rounded border-2 border-black px-4 py-2"
              : "rounded border px-4 py-2 hover:border-gray-500";

          return (
            <button
              key={variant.id}
              type="button"
              disabled={soldOut}
              aria-pressed={isSelected}
              onClick={() => setSelectedId(variant.id)}
              className={classes}
            >
              {variant.size}
            </button>
          );
        })}
      </div>

      {selected ? (
        <a
          href={buildWhatsAppUrl(whatsappNumber, {
            name: product.name,
            slug: product.slug,
            size: selected.size,
            price: selected.price,
            currencyCode,
          })}
          target="_blank"
          rel="noopener noreferrer"
          className="mt-4 inline-block rounded bg-green-600 px-6 py-3 font-semibold text-white"
        >
          Buy on WhatsApp
        </a>
      ) : (
        <p className="mt-4 text-sm text-gray-600">Select a size to continue</p>
      )}

      {selected && (
        <p className="mt-3">{formatPrice(selected.price, currencyCode)}</p>
      )}
    </section>
  );
}
