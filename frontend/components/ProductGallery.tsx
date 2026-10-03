"use client";

import { useState } from "react";
import type { ProductImageItem } from "@/lib/api";
import { ProductImage } from "@/components/ProductImage";

type Props = {
  name: string;
  images: ProductImageItem[];
  fallbackUrl: string;
};

export function ProductGallery({ name, images, fallbackUrl }: Props) {
  const items = images.length > 0 ? images : [{ url: fallbackUrl, altText: null }];
  const [activeIndex, setActiveIndex] = useState(0);
  const active = items[activeIndex] ?? items[0];

  return (
    <div className="product-gallery">
      <div className="product-detail-image">
        <ProductImage
          key={active.url}
          src={active.url}
          alt={active.altText ?? name}
          priority
        />
      </div>
      {items.length > 1 ? (
        <div className="product-gallery__thumbs" role="group" aria-label="Product images">
          {items.map((image, index) => (
            <button
              key={image.url}
              type="button"
              className={`product-gallery__thumb${index === activeIndex ? " is-active" : ""}`}
              aria-label={`Show image ${index + 1} of ${items.length}`}
              aria-pressed={index === activeIndex}
              onClick={() => setActiveIndex(index)}
            >
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img src={image.url} alt="" loading="lazy" />
            </button>
          ))}
        </div>
      ) : null}
    </div>
  );
}
