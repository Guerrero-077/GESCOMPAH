import { Routes } from "@angular/router";
import { ListEstablishmentComponent } from "./Pages/list-establishment-component/list-establishment-component";
import { MainComponent } from "./Pages/main-component/main-component";

export const ESTABLISHMENT_ROUTES: Routes = [
    { path: 'ManagementPremisesEstablishments', component: MainComponent },
    { path: 'ListEstablishment', component: ListEstablishmentComponent },
    // { path: 'ListContrato', component: ListEstablishmentComponent }
    // {path: 'register', component: RegisterComponent}
];
