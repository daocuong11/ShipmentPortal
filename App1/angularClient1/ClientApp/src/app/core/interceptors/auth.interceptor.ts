import { Injectable, Injector } from "@angular/core";
import { HttpInterceptor, HttpHandler, HttpEvent, HttpRequest, HttpHeaders } from "@angular/common/http";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { Observable } from "rxjs";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private _oidcSecurityService: OidcSecurityService;

    constructor(private injector: Injector) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(this._setHeaders(req));
    }

    private _setHeaders(req: HttpRequest<any>): HttpRequest<any> {
        let headers = req.headers
            //.set('Content-Type', 'application/json')
            .set('Accept', 'application/json');

        if (this._oidcSecurityService === undefined) {
            this._oidcSecurityService = this.injector.get(OidcSecurityService);
        }
        if (this._oidcSecurityService !== undefined) {
            const token = this._oidcSecurityService.getToken();
            if (token !== '') {
                headers = headers.set('Authorization', 'Bearer ' + token);
            }
        }

        return req.clone({
            headers: headers
        });
    }
}