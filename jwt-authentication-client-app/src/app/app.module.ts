import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpModule, XHRBackend } from "@angular/http";
import { HttpClientModule } from "@angular/common/http";
import { AuthenticateXHRBackend } from "./authenticate-xhr.backend";

import { routing } from "./app.routing";

/* App Root */
import { AppComponent } from "./app.component";
import { HeaderComponent } from "./header/header.component";
import { HomeComponent } from "./home/home.component";

/* Account Imports */
import { AccountModule } from "./account/account.module";
/* Dashboard Imports */
import { DashboardModule } from "./dashboard/dashboard.module";

import { ConfigService } from "./shared/utils/config.service";
import { UserService } from "./shared/services/user.service";

@NgModule({
  declarations: [AppComponent, HomeComponent, HeaderComponent],
  imports: [
    AccountModule,
    DashboardModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule,
    routing
  ],
  providers: [
    ConfigService,
    {
      provide: XHRBackend,
      useClass: AuthenticateXHRBackend
    },
    UserService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
