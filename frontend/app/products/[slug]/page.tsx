import Image from "next/image";
import { notFound } from "next/navigation";
import { getProductBySlug, getStorefront } from "@/lib/api";
import ProductPurchasePanel from "@/components/ProductPurchasePanel";

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
    <main className="mx-auto max-w-4xl px-6 py-10">
      <Image
        src={product.imageUrl}
        alt={product.name}
        width={800}
        height={1000}
        className="w-full max-w-md rounded-lg object-cover"
      />
      <h1 className="mt-6 text-3xl font-bold">{product.name}</h1>
      <p className="mt-1 text-gray-600">
        {product.club} · {product.season} · {product.kitType}
      </p>
      <p className="mt-4 max-w-prose">{product.description}</p>
      <ProductPurchasePanel
        product={product}
        whatsappNumber={storefront.whatsAppNumber}
        currencyCode={storefront.currencyCode}
      />
    </main>
  );
}
