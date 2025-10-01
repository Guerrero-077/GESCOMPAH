import { Routes } from "@angular/router";
import { LoginComponent } from "./pages/login/login.component";
// import { ResetPasswordComponent } from "./pages/reset-password/reset-password.component";
// import { RegisterComponent } from "./pages/register/register.component";

export const AUTH_ROUTES: Routes = [
    { path: 'login', component: LoginComponent },
    // { path: 'register', component: RegisterComponent },
    // { path: 'password_reset', component: ResetPasswordComponent},

    { path: '**', redirectTo: 'login' }
]
