export interface LoginModel {
    email: string;
    password: string;
}

export interface LoginResponseModel {
    id: string;
    email: string;
    roles: string[];
}