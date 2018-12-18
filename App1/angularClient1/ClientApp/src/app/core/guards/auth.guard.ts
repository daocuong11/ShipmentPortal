import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Injectable()
export class AuthorizationGuard implements CanActivate {

    constructor(
        private router: Router,
        private oidcSecurityService: OidcSecurityService
    ) { }

    public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | boolean {
        console.log(route + '' + state);
        console.log('AuthorizationGuard, canActivate');

        return this.oidcSecurityService.getIsAuthorized().pipe(
            map((isAuthorized: boolean) => {
                console.log('AuthorizationGuard, canActivate isAuthorized: ' + isAuthorized);

                if (isAuthorized) {
                    return true;
                }

                this.router.navigate(['/login']);
                return false;
            }),
            take(1)
        );
    }
}

