import { AdminProductsManager } from "@/components/AdminProductsManager";

export default function AdminProductsPage() {
  return (
    <main className="admin-main">
      <div className="admin-page-title">
        <div>
          <span className="section-eyebrow">Catalog operations</span>
          <h1>Products</h1>
          <p>Manage listings, variants, stock and product images.</p>
        </div>
      </div>
      <AdminProductsManager />
    </main>
  );
}
