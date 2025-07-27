export interface EstablishmentSelect {
    id: number;
    name: string;
    description: string;
    areaM2: number;
    rentValueBase: number;
    address: string;
    plazaId: number;
    plazaName: string;
    images: Image[];
    active: boolean;
}

export interface EstablishmentCreate {
    id: number;
    name: string;
    description: string;
    areaM2: number;
    rentValueBase: number;
    address: string;
    plazaId: number;
    files?: File[]; // ← ESTA es la clave para que funcione
}

export interface Image {
    id: number;
    fileName: string;
    filePath: string;
}
export interface ImageCreate {
    id: number;
    fileName: string;
    filePath: string;
}