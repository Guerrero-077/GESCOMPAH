import { Component } from "@angular/core";
import { LoginComponent } from "./pages/login/login.component";
import { Routes } from "@angular/router";

export const AUTH_ROUTES: Routes = [
    { path: 'login', component: LoginComponent }
]