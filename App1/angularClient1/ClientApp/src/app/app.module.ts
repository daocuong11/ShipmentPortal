import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import {
  AuthModule,
  OidcSecurityService,
  OpenIDImplicitFlowConfiguration,
  OidcConfigService,
  AuthWellKnownEndpoints
} from 'angular-auth-oidc-client';

import { environment } from '../environments/environment';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { UiModule } from './ui/ui.module';
import { routing } from './app.routes';

import { HomeComponent } from './home/home.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { LoginComponent } from './login/login.component';
import { IdentityComponent } from './identity/identity.component';

export function loadConfig(oidcConfigService: OidcConfigService) {
  console.log('APP_INITIALIZER STARTING');
  return () => oidcConfigService.load_using_stsServer(environment.loginUrl);
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ForbiddenComponent,
    UnauthorizedComponent,
    LoginComponent,
    IdentityComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    routing,
    AuthModule.forRoot(),
    CoreModule,
    UiModule.forRoot()
  ],
  providers: [
    OidcConfigService,
    OidcSecurityService,
    {
        provide: APP_INITIALIZER,
        useFactory: loadConfig,
        deps: [OidcConfigService],
        multi: true,
    }
  ],
  bootstrap: [AppComponent]
})

export class AppModule {
  constructor(
    private oidcSecurityService: OidcSecurityService,
    private oidcConfigService: OidcConfigService
  ) {
    this.oidcConfigService.onConfigurationLoaded.subscribe(() => {
      const openIDImplicitFlowConfiguration = new OpenIDImplicitFlowConfiguration();

      openIDImplicitFlowConfiguration.stsServer = environment.loginUrl;
      openIDImplicitFlowConfiguration.redirect_url = environment.spaUrl;
      // The Client MUST validate that the aud (audience) Claim contains its client_id value registered
      // at the Issuer identified by the iss (issuer) Claim as an audience.
      // The ID Token MUST be rejected if the ID Token does not list the Client as a valid audience,
      // or if it contains additional audiences not trusted by the Client.
      openIDImplicitFlowConfiguration.client_id = 'angularClient1';
      openIDImplicitFlowConfiguration.response_type = 'id_token token';
      openIDImplicitFlowConfiguration.scope = 'openid apitest1';
      openIDImplicitFlowConfiguration.post_logout_redirect_uri = `${environment.spaUrl}/unauthorized`;
      openIDImplicitFlowConfiguration.start_checksession = false;
      openIDImplicitFlowConfiguration.silent_renew = true;
      openIDImplicitFlowConfiguration.silent_renew_url = `${environment.spaUrl}/assets/static/silent-renew.html`;
      openIDImplicitFlowConfiguration.post_login_route = '/home';
      // HTTP 403
      openIDImplicitFlowConfiguration.forbidden_route = '/forbidden';
      // HTTP 401
      openIDImplicitFlowConfiguration.unauthorized_route = '/unauthorized';
      openIDImplicitFlowConfiguration.log_console_warning_active = true;
      openIDImplicitFlowConfiguration.log_console_debug_active = true;
      // id_token C8: The iat Claim can be used to reject tokens that were issued too far away from the current time,
      // limiting the amount of time that nonces need to be stored to prevent attacks.The acceptable range is Client specific.
      openIDImplicitFlowConfiguration.max_id_token_iat_offset_allowed_in_seconds = 300;
      openIDImplicitFlowConfiguration.auto_clean_state_after_authentication = false;

      const authWellKnownEndpoints = new AuthWellKnownEndpoints();
      authWellKnownEndpoints.setWellKnownEndpoints(this.oidcConfigService.wellKnownEndpoints);

      this.oidcSecurityService.setupModule(
        openIDImplicitFlowConfiguration,
        authWellKnownEndpoints
      );
    });

    console.log('APP STARTING');
  }
}
