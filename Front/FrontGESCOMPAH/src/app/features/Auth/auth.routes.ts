import { Routes } from "@angular/router";
import { LoginComponent } from "./pages/login/login-component/login-component";

export const AUTH_ROUTES: Routes = [
    { path: 'login', component: LoginComponent },
    // {path: 'register', component: RegisterComponent}
];