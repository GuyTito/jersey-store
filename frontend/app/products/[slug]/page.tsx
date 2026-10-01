import Link from "next/link";
import { notFound } from "next/navigation";
import { getProductBySlug, getStorefront } from "@/lib/api";
import { ProductImage } from "@/components/ProductImage";
import ProductPurchasePanel from "@/components/ProductPurchasePanel";
import { StorefrontHeader } from "@/components/StorefrontHeader";

export default async function ProductPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const [product, storefront] = await Promise.all([
    getProductBySlug(slug),
    getStorefront(),
  ]);

  if (!product) notFound();

  return (
    <div className="storefront-page">
      <StorefrontHeader />
      <main className="site-container product-page">
        <Link href="/" className="product-back">
          ← Back to all jerseys
        </Link>
        <div className="product-detail-grid">
          <div className="product-detail-image">
            <ProductImage src={product.imageUrl} alt={product.name} priority />
          </div>
          <div className="product-detail-copy">
            <span className="section-eyebrow">{product.club}</span>
            <h1>{product.name}</h1>
            <p className="product-detail-meta">
              {product.season} · {product.kitType}
            </p>
            <p className="product-detail-description">{product.description}</p>
            <ProductPurchasePanel
              product={product}
              whatsappNumber={storefront.whatsAppNumber}
              currencyCode={storefront.currencyCode}
            />
          </div>
        </div>
      </main>
    </div>
  );
}
