// export interface EstablishmentSelect {
//     id: number;
//     name: string;
//     description: string;
//     areaM2: number;
//     rentValueBase: number;
//     address: string;
//     plazaId: number;
//     plazaName: string;
//     images: Image[];
//     active: boolean;
// }

// export interface EstablishmentCreate {
//     id: number;
//     name: string;
//     description: string;
//     areaM2: number;
//     rentValueBase: number;
//     address: string;
//     plazaId: number;
//     files?: File[]; // ← ESTA es la clave para que funcione
// }
// export interface EstablishmentUpdate {
//     id: number;
//     name: string;
//     description: string;
//     areaM2: number;
//     rentValueBase: number;
//     plazaId: number;

//     // Archivos nuevos que el usuario sube en la edición (máx. 5 en total incluyendo existentes)
//     files?: File[];

//     // Opcional: imágenes que ya existen en Cloudinary (solo de lectura en UI)
//     existingImages?: ExistingImage[];
// }

// // Útil si necesitas renderizar en UI las imágenes actuales
// export interface ExistingImage {
//     id: number;
//     fileName: string;
//     filePath: string;
//     publicId: string;
// }


// export interface Image {
//     id: number;
//     fileName: string;
//     filePath: string;
// }
// export interface ImageCreate {
//     id: number;
//     fileName: string;
//     filePath: string;
// }







// export interface EstablishmentSelect {
//     id: number;
//     name: string;
//     description: string;
//     areaM2: number;
//     rentValueBase: number;
//     address: string;
//     plazaId: number;
//     plazaName: string;
//     images: Image[];        // Imágenes ya cargadas en el sistema
//     active: boolean;
//     // Puedes agregar más campos según necesites
// }

// export interface EstablishmentCreate {
//     name: string;
//     description: string;
//     areaM2: number;
//     rentValueBase: number;
//     plazaId: number;
//     files?: File[];         // Nuevas imágenes a subir (máx. 5)
//     address?: string;       // Hice este campo opcional si no es requerido
// }

// export interface EstablishmentUpdate {
//     id: number;
//     name: string;
//     description: string;
//     areaM2: number;
//     rentValueBase: number;
//     plazaId: number;
//     address?: string;

//     files?: File[]; // Nuevas imágenes a subir
//     existingImages?: ExistingImage[]; // Imágenes existentes que se mantienen
//     imagesToDelete?: string[]; // PublicIds de imágenes que el usuario quiere eliminar
// }
// export interface ExistingImage {
//     id: number;             // ID de la imagen en la base de datos
//     fileName: string;       // Nombre original del archivo
//     filePath: string;       // URL o path de la imagen
//     publicId: string;       // Identificador en Cloudinary o similar
// }

// export interface Image {
//     id: number;
//     fileName: string;
//     filePath: string;
//     publicId?: string;      // Hice este campo opcional para flexibilidad
// }

// export interface ImageSelectDto {
//     id: number;
//     fileName: string;
//     filePath: string;
//     publicId: string;
//     establishmentId: number;
// }

// export interface ImageCreateDto {
//     establishmentId: number;
//     files: File[];
// }

// export interface ImageUpdateDto {
//     id: number;
//     fileName?: string;
//     filePath?: string;
//     publicId?: string;
// }






// ---------------------------------------------------------------------
// 4.  Modelos del cliente – DTOs
// ---------------------------------------------------------------------

export interface EstablishmentSelect {
    id: number;
    name: string;
    description: string;
    areaM2: number;
    rentValueBase: number;
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
