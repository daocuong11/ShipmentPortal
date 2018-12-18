import { Routes, RouterModule } from '@angular/router';

import { ForbiddenComponent } from './forbidden/forbidden.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { AuthorizationGuard } from './auth.guard';
import { AuthorizationCanGuard } from './auth.can.guard';
import { LoginComponent } from './login/login.component';
import { IdentityComponent } from './identity/identity.component';

const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'home', component: HomeComponent, canActivate: [AuthorizationGuard], canLoad: [AuthorizationCanGuard] },
    { path: 'identity', component: IdentityComponent, canActivate: [AuthorizationGuard], canLoad: [AuthorizationCanGuard] },
    { path: 'login', component: LoginComponent },
    { path: 'forbidden', component: ForbiddenComponent },
    { path: 'unauthorized', component: UnauthorizedComponent }
];

export const routing = RouterModule.forRoot(appRoutes);
