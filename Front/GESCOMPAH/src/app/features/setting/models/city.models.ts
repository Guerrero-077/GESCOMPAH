export interface CitySelectModel {
    id: number;
    name: string;
    departmentName: string;
    active: boolean;
}

export interface CityCreate {
    name: string;
    departmentId: number;
}

export interface CityUpdate {
    id: number;
    name: string;
    departmentId: number;
    active: boolean;
}
