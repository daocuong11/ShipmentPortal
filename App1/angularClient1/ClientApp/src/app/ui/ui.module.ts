import { NgModule, ModuleWithProviders } from "@angular/core";
import { CommonModule } from "@angular/common";

/*
    Here is the place to put Reusable UI components
*/

@NgModule({
    declarations: [],
    imports: [],
    exports: [
        CommonModule
    ]
})
export class UiModule {
    static forRoot(): ModuleWithProviders  {
        return {
            ngModule: UiModule,
            providers: [
                // Provide services needed by UI module itself
            ]
        };
    }
}