import { Component } from "@angular/core";

import { UserService } from "../../shared/services/user.service";
import { Router } from "@angular/router";

@Component({
  selector: "app-linkedin-login",
  templateUrl: "./linkedin-login.component.html",
  styleUrls: ["./linkedin-login.component.css"]
})
export class LinkedinLoginComponent {
  private authWindow: Window;
  failed: boolean;
  error: string;
  errorDescription: string;
  isRequesting: boolean;

  launchLinkedInLogin() {
    this.authWindow = window.open(
      "https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id=<your-linkedin-registered-client-app-id>&redirect_uri=https%3A%2F%2Flocalhost%3A44364%2Fapi%2Fexternalauth%2Fcallback&state=12345&scope=r_basicprofile,r_emailaddress,rw_company_admin,w_share"
    );
  }

  constructor(private userService: UserService, private router: Router) {
    if (window.addEventListener) {
      window.addEventListener("message", this.handleMessage.bind(this), false);
    } else {
      (<any>window).attachEvent("onmessage", this.handleMessage.bind(this));
    }
  }

  handleMessage(event: Event) {
    if (this.authWindow) {
      const message = event as MessageEvent;

      // Only trust messages from the below origin.
      if (message.origin !== "http://localhost:4200") {
        return;
      }

      this.authWindow.close();

      const result = JSON.parse(message.data);

      if (!result.status) {
        this.failed = true;
        this.error = result.error;
        this.errorDescription = result.errorDescription;
      } else {
        this.failed = false;
        this.isRequesting = true;

        this.userService
          .linkedInLogin(result.accessToken)
          .subscribe(
            result => {
              if (result) {
                this.router.navigate(["/dashboard/home"]);
              }
            },
            error => {
              this.failed = true;
              this.error = error;
            }
          )
          .add(() => (this.isRequesting = false));
      }
    }
  }
}
