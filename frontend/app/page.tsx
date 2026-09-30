import Image from "next/image";
import Link from "next/link";
import { getProducts } from "@/lib/api";
import { formatPrice } from "@/lib/format";

export default async function Home() {
  const products = await getProducts();

  return (
    <main className="mx-auto max-w-6xl px-6 py-10">
      <header className="mb-8">
        <h1 className="text-3xl font-bold">Jersey Store</h1>
        <p className="mt-1 text-gray-600">Authentic club jerseys, ordered over WhatsApp.</p>
      </header>

      <ul className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        {products.map((product) => (
          <li key={product.id}>
            <Link href={`/products/${product.slug}`} className="group block">
              <Image
                src={product.imageUrl}
                alt={product.name}
                width={800}
                height={1000}
                className="aspect-[4/5] w-full rounded-lg object-cover"
              />
              <h2 className="mt-3 font-semibold group-hover:underline">{product.name}</h2>
              <p className="text-sm text-gray-600">
                {product.club} · {product.season} · {product.kitType}
              </p>
              <p className="mt-1 font-medium">
                {product.lowestInStockPrice === null
                  ? "Sold out"
                  : formatPrice(product.lowestInStockPrice)}
              </p>
            </Link>
          </li>
        ))}
      </ul>
    </main>
  );
}