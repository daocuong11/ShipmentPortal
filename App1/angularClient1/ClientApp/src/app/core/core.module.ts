import { NgModule, Optional, SkipSelf } from "@angular/core";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { AuthorizationGuard, AuthorizationCanGuard } from "./guards";
import { AuthInterceptor } from "./interceptors";

/*
    Here is the place to put Global and Singleton Service
*/

@NgModule({
    declarations: [],
    imports: [],
    providers: [
    AuthorizationGuard,
    AuthorizationCanGuard,
    {
        provide: HTTP_INTERCEPTORS,
        useClass: AuthInterceptor,
        multi: true,
    }
    ],
    exports: []
})
export class CoreModule {
    constructor(@Optional() @SkipSelf() core: CoreModule) {
        if (core) {
            throw new Error("Core module is imported more than once!!!");
        }
    }
}