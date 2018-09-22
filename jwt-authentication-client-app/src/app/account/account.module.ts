import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { SharedModule } from "../shared/modules/shared.module";
import { UserService } from "../shared/services/user.service";
import { EmailValidator } from "../directives/email.validator.directive";
import { routing } from "./account.routing";
import { RegistrationFormComponent } from "./registration-form/registration-form.component";
import { LoginFormComponent } from "./login-form/login-form.component";
import { LinkedinLoginComponent } from "./linkedin-login/linkedin-login.component";
import { SpinnerComponent } from "../spinner/spinner.component";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    routing,
    SharedModule,
    ReactiveFormsModule
  ],
  declarations: [
    RegistrationFormComponent,
    EmailValidator,
    LoginFormComponent,
    LinkedinLoginComponent,
    SpinnerComponent
  ],
  providers: [UserService]
})
export class AccountModule {}
