import { Routes } from '@angular/router';
import { CityComponent } from './components/city/city.component';
import { DepartmentComponent } from './components/department/department.component';
import { MainSettingsComponent } from './pages/main-settings/main-settings.component';

export const SETTING_ROUTES: Routes = [
  { path: 'main', component: MainSettingsComponent }
];
