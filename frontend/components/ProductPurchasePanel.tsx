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
      <p className="sold-out-message" role="status">
        This jersey is sold out.
      </p>
    );
  }

  return (
    <section className="purchase-panel">
      <h2 className="purchase-panel__title">Choose a size</h2>
      <div className="size-options" role="group" aria-label="Choose a size">
        {product.variants.map((variant) => {
          const soldOut = variant.stock === 0;
          const isSelected = variant.id === selectedId;
          const classes = [
            "size-option",
            soldOut && "size-option--sold-out",
            isSelected && "is-selected",
          ]
            .filter(Boolean)
            .join(" ");

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
        <>
          <p className="purchase-price">
            {formatPrice(selected.price, currencyCode)}{" "}
            <span>for size {selected.size}</span>
          </p>
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
            className="purchase-order"
          >
            Order on WhatsApp <span aria-hidden="true">↗</span>
          </a>
        </>
      ) : (
        <p className="purchase-hint">Select a size to see its price.</p>
      )}
    </section>
  );
}
