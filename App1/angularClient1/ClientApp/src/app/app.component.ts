import { Component, OnInit, OnDestroy } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Subscription } from 'rxjs';
import { filter, take } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'App 1';
  isAuthorized;
  isAuthorizedSubscription: Subscription | undefined;

  constructor(public oidcSecurityService: OidcSecurityService) {
    this.oidcSecurityService.getIsModuleSetup().pipe(
      filter((isModuleSetup: boolean) => isModuleSetup),
      take(1)
    ).subscribe((isModuleSetup: boolean) => {
      this.doCallbackLogicIfRequired();
    });
  }

  ngOnInit() {
    this.isAuthorizedSubscription = this.oidcSecurityService.getIsAuthorized().subscribe(
      (isAuthorized: boolean) => {
        this.isAuthorized = isAuthorized;
      });
  }

  ngOnDestroy(): void {
    if (this.isAuthorizedSubscription) {
      this.isAuthorizedSubscription.unsubscribe();
    }
  }

  login() {
    this.authorize('ex');
  }

  loginInternal() {
    this.authorize('in');
  }

  logout() {
    this.oidcSecurityService.logoff();
  }

  private authorize(type: string) {
    this.oidcSecurityService.authorize(url => {
      const connector = url.includes('?') ? '&' : '?';
      window.location.href = `${url}${connector}type=${type}`;
    });
  }

  private doCallbackLogicIfRequired() {
    if (window.location.hash) {
      this.oidcSecurityService.authorizedCallback();
    }
  }
}
