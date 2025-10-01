
export interface EstablishmentSelect {
  id: number;
  name: string;
  description: string;
  areaM2: number;
  rentValueBase: number;
  uvtQty: number;
  address: string;
  plazaId: number;
  plazaName: string;
  images: ImageSelectDto[];
  active: boolean;
}

export interface EstablishmentCreate {
  name: string;
  description: string;
  areaM2: number;
  rentValueBase: number;
  uvtQty: number;
  plazaId: number;
  address?: string;
  files?: File[];          // <-- la colección de archivos nuevos (máx 5)
}

/**
 * Cuando se actualiza, se puede enviar:
 *  - imágenes nuevas (`images` – File[])
 *  - publicIds a borrar (`imagesToDelete`)
 */
export interface EstablishmentUpdate {
  id: number;
  name?: string;
  description?: string;
  areaM2?: number;
  rentValueBase?: number;
  uvtQty?: number;
  plazaId?: number;
  address?: string;

  images?: File[];                    // archivos nuevos
  imagesToDelete?: string[];          // publicIds a eliminar
}

/** Imagen que el API devuelve en la salida. */
export interface ImageSelectDto {
  id: number;
  fileName: string;
  filePath: string;
  publicId: string;
  establishmentId: number;
}


export type PreviewData = {
  sources: string[];   // rutas o dataURLs
  index: number;       // índice inicial
  title?: string;      // opcional
};
