"use client";

import Image from "next/image";
import { FormEvent, useCallback, useEffect, useState } from "react";
import { useRouter } from "next/navigation";

type AdminVariant = {
  id: number;
  size: string;
  sizeOrder: number;
  price: number;
  stock: number;
  sku: string | null;
  isActive: boolean;
};

type AdminImage = {
  id: number;
  url: string;
  altText: string | null;
  isPrimary: boolean;
  displayOrder: number;
};

type AdminProduct = {
  id: number;
  slug: string;
  name: string;
  club: string;
  season: string;
  kitType: string;
  description: string;
  displayOrder: number;
  isActive: boolean;
  variants: AdminVariant[];
  images: AdminImage[];
};

type VariantForm = {
  clientKey: string;
  id?: number;
  size: string;
  sizeOrder: number;
  price: number;
  stock: number;
  sku: string;
  isActive: boolean;
};

type ProductForm = {
  slug: string;
  name: string;
  club: string;
  season: string;
  kitType: string;
  description: string;
  displayOrder: number;
  isActive: boolean;
  variants: VariantForm[];
};

let variantKeyCounter = 0;
const newVariantKey = () => `variant-${++variantKeyCounter}`;

const emptyForm = (): ProductForm => ({
  slug: "",
  name: "",
  club: "",
  season: "",
  kitType: "Home",
  description: "",
  displayOrder: 0,
  isActive: true,
  variants: [{ clientKey: newVariantKey(), size: "M", sizeOrder: 2, price: 0, stock: 0, sku: "", isActive: true }],
});

function fromProduct(product: AdminProduct): ProductForm {
  return {
    slug: product.slug,
    name: product.name,
    club: product.club,
    season: product.season,
    kitType: product.kitType,
    description: product.description,
    displayOrder: product.displayOrder,
    isActive: product.isActive,
    variants: product.variants.map((variant) => ({
      ...variant,
      clientKey: newVariantKey(),
      sku: variant.sku ?? "",
    })),
  };
}

function errorMessage(body: unknown, fallback: string) {
  if (body && typeof body === "object") {
    const values = body as Record<string, unknown>;
    for (const key of ["error", "detail", "title"]) {
      if (typeof values[key] === "string") return values[key];
    }
  }
  return fallback;
}

async function requestError(
  response: Response,
  fallback: string,
  onUnauthorized: () => void,
) {
  if (response.status === 401) {
    onUnauthorized();
  }

  let body: unknown;
  try {
    body = await response.json();
  } catch {
    body = null;
  }
  return errorMessage(body, `${fallback} (HTTP ${response.status}).`);
}

function imageSource(url: string) {
  if (!url.startsWith("/uploads/")) return url;
  return `/api/product-images/${encodeURIComponent(url.slice("/uploads/".length))}`;
}

export function AdminProductsManager() {
  const router = useRouter();
  const [products, setProducts] = useState<AdminProduct[]>([]);
  const [form, setForm] = useState<ProductForm>(emptyForm);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [stockDrafts, setStockDrafts] = useState<Record<number, string>>({});
  const [selectedImage, setSelectedImage] = useState<File | null>(null);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");

  const readError = useCallback(
    (response: Response, fallback: string) =>
      requestError(response, fallback, () => router.replace("/admin/login")),
    [router],
  );

  const fetchProducts = useCallback(async () => {
    const response = await fetch("/api/admin/proxy/products", { cache: "no-store" });
    if (!response.ok) {
      throw new Error(await readError(response, "Could not load products."));
    }
    return (await response.json()) as AdminProduct[];
  }, [readError]);

  useEffect(() => {
    let active = true;
    fetchProducts()
      .then((result) => {
        if (active) setProducts(result);
      })
      .catch((loadError: unknown) => {
        if (!active) return;
        setError(loadError instanceof Error ? loadError.message : "Could not load products.");
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, [fetchProducts]);

  const normalizedSearch = search.trim().toLowerCase();
  const visibleProducts = normalizedSearch
    ? products.filter((product) =>
        [product.name, product.club, product.season, product.kitType, product.slug]
          .some((value) => value.toLowerCase().includes(normalizedSearch)),
      )
    : products;

  function beginCreate() {
    setEditingId(null);
    setForm(emptyForm());
    setSelectedImage(null);
    setError("");
    setNotice("");
  }

  function beginEdit(product: AdminProduct) {
    setEditingId(product.id);
    setForm(fromProduct(product));
    setSelectedImage(null);
    setError("");
    setNotice("");
  }

  function updateField<Key extends keyof ProductForm>(key: Key, value: ProductForm[Key]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function updateVariant(index: number, patch: Partial<VariantForm>) {
    setForm((current) => ({
      ...current,
      variants: current.variants.map((variant, variantIndex) =>
        variantIndex === index ? { ...variant, ...patch } : variant,
      ),
    }));
  }

  async function saveProduct(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setBusy(true);
    setError("");
    setNotice("");
    try {
      const response = await fetch(
        editingId === null
          ? "/api/admin/proxy/products"
          : `/api/admin/proxy/products/${editingId}`,
        {
          method: editingId === null ? "POST" : "PUT",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(form),
        },
      );
      if (!response.ok) {
        throw new Error(await readError(response, "Could not save product."));
      }

      const saved = (await response.json()) as AdminProduct;
      setEditingId(saved.id);
      setForm(fromProduct(saved));
      setProducts(await fetchProducts());
      setNotice("Product saved.");
    } catch (saveError) {
      setError(saveError instanceof Error ? saveError.message : "Could not save product.");
    } finally {
      setBusy(false);
    }
  }

  async function deleteProduct(product: AdminProduct) {
    if (!window.confirm(`Delete "${product.name}"? This cannot be undone.`)) return;

    setBusy(true);
    setError("");
    setNotice("");
    try {
      const response = await fetch(`/api/admin/proxy/products/${product.id}`, {
        method: "DELETE",
      });
      if (!response.ok) {
        throw new Error(await readError(response, "Could not delete product."));
      }
      if (editingId === product.id) beginCreate();
      setProducts(await fetchProducts());
      setNotice("Product deleted.");
    } catch (deleteError) {
      setError(deleteError instanceof Error ? deleteError.message : "Could not delete product.");
    } finally {
      setBusy(false);
    }
  }

  async function updateStock(product: AdminProduct, variant: AdminVariant) {
    const value = Number(stockDrafts[variant.id] ?? variant.stock);
    if (!Number.isSafeInteger(value) || value < 0) {
      setError("Stock must be a non-negative whole number.");
      return;
    }

    setBusy(true);
    setError("");
    setNotice("");
    try {
      const response = await fetch(
        `/api/admin/proxy/products/${product.id}/variants/${variant.id}/stock`,
        {
          method: "PATCH",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ stock: value }),
        },
      );
      if (!response.ok) {
        throw new Error(await readError(response, "Could not update stock."));
      }
      setProducts(await fetchProducts());
      setNotice(`Stock updated for ${product.name}, size ${variant.size}.`);
    } catch (stockError) {
      setError(stockError instanceof Error ? stockError.message : "Could not update stock.");
    } finally {
      setBusy(false);
    }
  }

  async function removeImage(imageId: number) {
    if (editingId === null || !window.confirm("Remove this image permanently?")) return;

    setBusy(true);
    setError("");
    setNotice("");
    try {
      const response = await fetch(
        `/api/admin/proxy/products/${editingId}/images/${imageId}`,
        { method: "DELETE" },
      );
      if (!response.ok) {
        throw new Error(await readError(response, "Could not remove the image."));
      }
      setProducts(await fetchProducts());
      setNotice("Image removed.");
    } catch (removeError) {
      setError(removeError instanceof Error ? removeError.message : "Could not remove the image.");
    } finally {
      setBusy(false);
    }
  }

  async function setPrimaryImage(imageId: number) {
    if (editingId === null) return;

    setBusy(true);
    setError("");
    setNotice("");
    try {
      const response = await fetch(
        `/api/admin/proxy/products/${editingId}/images/${imageId}/primary`,
        { method: "PUT" },
      );
      if (!response.ok) {
        throw new Error(await readError(response, "Could not set the primary image."));
      }
      setProducts(await fetchProducts());
      setNotice("Primary image updated.");
    } catch (primaryError) {
      setError(
        primaryError instanceof Error ? primaryError.message : "Could not set the primary image.",
      );
    } finally {
      setBusy(false);
    }
  }

  async function uploadImage() {
    if (editingId === null || !selectedImage) return;

    const data = new FormData();
    data.set("file", selectedImage);
    data.set("altText", `${form.name} product image`);
    setBusy(true);
    setError("");
    setNotice("");
    try {
      const response = await fetch(`/api/admin/proxy/products/${editingId}/images`, {
        method: "POST",
        body: data,
      });
      if (!response.ok) {
        throw new Error(await readError(response, "Could not upload image."));
      }
      setSelectedImage(null);
      setProducts(await fetchProducts());
      setNotice("Image uploaded.");
    } catch (uploadError) {
      setError(uploadError instanceof Error ? uploadError.message : "Could not upload image.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="admin-grid">
      <section className="admin-panel">
        <div className="admin-panel__heading">
          <div>
            <h2>Catalog</h2>
            <p>
              {normalizedSearch
                ? `${visibleProducts.length} of ${products.length} products`
                : `${products.length} products, including inactive listings`}
            </p>
          </div>
          <button className="button button--lime" type="button" onClick={beginCreate}>
            Add product
          </button>
        </div>
        <input
          type="search"
          className="admin-search"
          placeholder="Search by name, club, season, or slug"
          aria-label="Search products"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
        {loading ? <p className="admin-state">Loading products…</p> : null}
        {!loading && products.length > 0 && visibleProducts.length === 0 ? (
          <p className="admin-state">No products match &ldquo;{search.trim()}&rdquo;.</p>
        ) : null}
        {!loading && products.length === 0 ? (
          <p className="admin-state">No products yet. Add the first catalog item.</p>
        ) : null}
        <div className="admin-product-list">
          {visibleProducts.map((product) => (
            <article className="admin-product" key={product.id}>
              <div className="admin-product__summary">
                <div>
                  <span className={`admin-status ${product.isActive ? "is-active" : ""}`}>
                    {product.isActive ? "Active" : "Inactive"}
                  </span>
                  <h3>{product.name}</h3>
                  <p>{product.club} · {product.season} · {product.slug}</p>
                </div>
                <div className="admin-product__actions">
                  <button type="button" className="admin-link-button" onClick={() => beginEdit(product)}>
                    Edit
                  </button>
                  <button
                    type="button"
                    className="admin-link-button is-danger"
                    disabled={busy}
                    onClick={() => deleteProduct(product)}
                  >
                    Delete
                  </button>
                </div>
              </div>
              <div className="admin-stock-list">
                {product.variants.map((variant) => (
                  <div className="admin-stock-row" key={variant.id}>
                    <span>{variant.size} · {variant.isActive ? "Available" : "Inactive"}</span>
                    <label>
                      <span className="sr-only">Stock for {product.name}, size {variant.size}</span>
                      <input
                        type="number"
                        min="0"
                        step="1"
                        value={stockDrafts[variant.id] ?? variant.stock}
                        onChange={(event) =>
                          setStockDrafts((current) => ({
                            ...current,
                            [variant.id]: event.target.value,
                          }))
                        }
                      />
                    </label>
                    <button
                      type="button"
                      className="admin-link-button"
                      disabled={busy || Number(stockDrafts[variant.id] ?? variant.stock) === variant.stock}
                      onClick={() => updateStock(product, variant)}
                    >
                      Save stock
                    </button>
                  </div>
                ))}
              </div>
            </article>
          ))}
        </div>
      </section>

      <section className="admin-panel admin-editor">
        <div className="admin-panel__heading">
          <div>
            <h2>{editingId === null ? "New product" : "Edit product"}</h2>
            <p>Product details and sale variants</p>
          </div>
          {editingId !== null ? (
            <button type="button" className="admin-link-button" onClick={beginCreate}>
              New
            </button>
          ) : null}
        </div>
        {error ? <p className="admin-error" role="alert">{error}</p> : null}
        {notice ? <p className="admin-notice" role="status">{notice}</p> : null}

        <form className="admin-form" onSubmit={saveProduct}>
          <div className="admin-form__row">
            <label>
              Product name
              <input required maxLength={200} value={form.name} onChange={(event) => updateField("name", event.target.value)} />
            </label>
            <label>
              Slug
              <input
                required
                maxLength={160}
                pattern="[a-z0-9]+(?:-[a-z0-9]+)*"
                value={form.slug}
                onChange={(event) => updateField("slug", event.target.value)}
              />
            </label>
          </div>
          <div className="admin-form__row">
            <label>
              Club
              <input required maxLength={120} value={form.club} onChange={(event) => updateField("club", event.target.value)} />
            </label>
            <label>
              Season
              <input required maxLength={20} value={form.season} onChange={(event) => updateField("season", event.target.value)} />
            </label>
          </div>
          <div className="admin-form__row">
            <label>
              Kit type
              <input required maxLength={40} value={form.kitType} onChange={(event) => updateField("kitType", event.target.value)} />
            </label>
            <label>
              Display order
              <input
                type="number"
                min="0"
                step="1"
                value={form.displayOrder}
                onChange={(event) => updateField("displayOrder", Number(event.target.value))}
              />
            </label>
          </div>
          <label>
            Description
            <textarea
              required
              maxLength={2000}
              rows={3}
              value={form.description}
              onChange={(event) => updateField("description", event.target.value)}
            />
          </label>
          <label className="admin-checkbox">
            <input
              type="checkbox"
              checked={form.isActive}
              onChange={(event) => updateField("isActive", event.target.checked)}
            />
            Show in public catalog
          </label>

          <div className="admin-variants-heading">
            <h3>Variants</h3>
            <button
              type="button"
              className="admin-link-button"
              onClick={() =>
                updateField("variants", [
                  {
                    clientKey: newVariantKey(),
                    size: "",
                    sizeOrder: form.variants.length + 1,
                    price: 0,
                    stock: 0,
                    sku: "",
                    isActive: true,
                  },
                  ...form.variants,
                ])
              }
            >
              Add size
            </button>
          </div>
          {form.variants.map((variant, index) => (
            <div className="admin-variant-form" key={variant.clientKey}>
              <label>
                Size
                <input required maxLength={20} value={variant.size} onChange={(event) => updateVariant(index, { size: event.target.value })} />
              </label>
              <label>
                Price
                <input type="number" min="0" max="999999.99" step="0.01" required value={variant.price} onChange={(event) => updateVariant(index, { price: Number(event.target.value) })} />
              </label>
              <label>
                Stock
                <input type="number" min="0" step="1" required value={variant.stock} onChange={(event) => updateVariant(index, { stock: Number(event.target.value) })} />
              </label>
              <label>
                SKU
                <input maxLength={64} value={variant.sku} onChange={(event) => updateVariant(index, { sku: event.target.value })} />
              </label>
              <label className="admin-checkbox">
                <input type="checkbox" checked={variant.isActive} onChange={(event) => updateVariant(index, { isActive: event.target.checked })} />
                Active
              </label>
              {form.variants.length > 1 ? (
                <button
                  type="button"
                  className="admin-link-button is-danger"
                  onClick={() => updateField("variants", form.variants.filter((_, variantIndex) => variantIndex !== index))}
                >
                  Remove
                </button>
              ) : null}
            </div>
          ))}
          <button className="button button--lime" disabled={busy}>
            {busy ? "Saving…" : editingId === null ? "Create product" : "Save changes"}
          </button>
        </form>

        {editingId !== null ? (
          <div className="admin-image-editor">
            <h3>Product images</h3>
            <div className="admin-image-list">
              {products.find((product) => product.id === editingId)?.images.map((image) => (
                <figure key={image.id} className="admin-image-item">
                  <Image
                    src={imageSource(image.url)}
                    alt={image.altText ?? form.name}
                    width={120}
                    height={150}
                  />
                  {image.isPrimary ? (
                    <figcaption className="admin-image-item__badge">Primary</figcaption>
                  ) : (
                    <button
                      type="button"
                      className="button button--outline admin-image-item__action"
                      disabled={busy}
                      onClick={() => setPrimaryImage(image.id)}
                    >
                      Set as primary
                    </button>
                  )}
                  <button
                    type="button"
                    className="admin-link-button admin-image-item__remove"
                    disabled={busy}
                    onClick={() => removeImage(image.id)}
                  >
                    Remove
                  </button>
                </figure>
              ))}
            </div>
            <label>
              Upload PNG, JPEG, or WebP (max 5 MB)
              <input
                type="file"
                accept="image/png,image/jpeg,image/webp"
                onChange={(event) => setSelectedImage(event.target.files?.[0] ?? null)}
              />
            </label>
            <button
              type="button"
              className="button button--outline"
              disabled={busy || selectedImage === null}
              onClick={uploadImage}
            >
              {busy ? "Uploading…" : "Upload image"}
            </button>
          </div>
        ) : (
          <p className="admin-help">Save the product first to attach images.</p>
        )}
      </section>
    </div>
  );
}
