export interface TenantsSelectModel {
    personName: string;
    email: string;
    active: boolean;
}
export interface TenantsCreateModel {
    email: string;
}
export interface TenantsUpdateModel {
    personId: number;
    email: string;
    active: boolean;
}
