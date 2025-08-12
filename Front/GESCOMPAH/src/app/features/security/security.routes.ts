import { Routes } from '@angular/router';
import { ModelSecurityComponent } from './pages/model-security/model-security.component';
import { RoleComponent } from './components/role/role.component';
import { FormComponent } from './components/form/form.component';
import { UserComponent } from './components/user/user.component';

export const SECURITY_ROUTES: Routes = [
  { path: 'main', component: ModelSecurityComponent },
  { path: 'users', component: UserComponent }
];
