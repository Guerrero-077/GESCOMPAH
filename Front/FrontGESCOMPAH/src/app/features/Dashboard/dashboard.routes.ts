import { Routes } from "@angular/router";
import { LocalesListComponent } from "./Components/Locales-list/locales-list-component/locales-list-component";

export const DASHBOARD_ROUTES: Routes = [
    { path: 'locales', component: LocalesListComponent },
    // {path: 'register', component: RegisterComponent}
];