import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { IdentityService } from './identity.service';

@Component({
    selector: 'app-identity',
    templateUrl: 'identity.component.html'
})

export class IdentityComponent implements OnInit, OnDestroy {

    message: string;
    identity: any[];

    constructor(private oidcSecurityService: OidcSecurityService,
        private _identityService: IdentityService) {
        this.message = 'IdentityComponent constructor';
    }

    ngOnInit() {
        this._identityService.get()
        .subscribe(i => {
            this.identity = i;
        });
    }

    ngOnDestroy(): void {
    }
}
