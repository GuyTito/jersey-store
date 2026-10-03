import type { Metadata } from "next";
import Link from "next/link";
import { notFound } from "next/navigation";
import { getProductBySlug, getStorefront } from "@/lib/api";
import { ProductGallery } from "@/components/ProductGallery";
import ProductPurchasePanel from "@/components/ProductPurchasePanel";
import { StorefrontHeader } from "@/components/StorefrontHeader";
import { WhatsAppButton } from "@/components/WhatsAppButton";

export async function generateMetadata({
  params,
}: {
  params: Promise<{ slug: string }>;
}): Promise<Metadata> {
  const { slug } = await params;
  const product = await getProductBySlug(slug);
  if (!product) return { title: "Jersey not found" };

  return {
    title: product.name,
    description: `${product.name} (${product.season} ${product.kitType}) from ${product.club}. Order over WhatsApp.`,
  };
}

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
      <WhatsAppButton />
      <main className="site-container product-page">
        <Link href="/" className="product-back">
          ← Back to all jerseys
        </Link>
        <div className="product-detail-grid">
          <ProductGallery name={product.name} images={product.images} fallbackUrl={product.imageUrl} />
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
