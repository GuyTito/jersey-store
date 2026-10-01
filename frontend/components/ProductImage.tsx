import Image from "next/image";

type Props = {
  src: string;
  alt: string;
  priority?: boolean;
};

export function ProductImage({ src, alt, priority = false }: Props) {
  if (src.startsWith("https://placehold.co/")) {
    return (
      <div className="jersey-placeholder" role="img" aria-label={alt}>
        <span className="jersey-placeholder__shirt" aria-hidden="true" />
      </div>
    );
  }

  return (
    <Image
      src={src}
      alt={alt}
      width={800}
      height={1000}
      priority={priority}
      className="product-card__image"
    />
  );
}
