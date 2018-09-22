import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { UserRegistration } from '../models/user.registration.interface';
import { ConfigService } from '../utils/config.service';

import { BaseService } from "./base.service";

import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs'; 
import { tap, catchError } from 'rxjs/operators';

@Injectable()
export class UserService extends BaseService {

    baseUrl: string = '';
  
    // Observable navItem source
    private _authNavStatusSource = new BehaviorSubject<boolean>(false);
    // Observable navItem stream
    authNavStatus$ = this._authNavStatusSource.asObservable();
  
    private loggedIn = false;
  
    constructor(private configService: ConfigService, private httpClient: HttpClient) {
      super();
      this.loggedIn = !!localStorage.getItem('auth_token');
      // ?? not sure if this the best way to broadcast the status but seems to resolve issue on page refresh where auth status is lost in
      // header component resulting in authed user nav links disappearing despite the fact user is still logged in
      this._authNavStatusSource.next(this.loggedIn);
      this.baseUrl = configService.getApiURI();
    }
  
    register(email: string, password: string, firstName: string, lastName: string,location: string): Observable<UserRegistration> {
        
        let body = JSON.stringify({ email, password, firstName, lastName,location });
        
        const httpOptions = {
            headers: new HttpHeaders({ 'Content-Type': 'application/json' })
        };
        
        return this.httpClient
                        .post(this.baseUrl + "/accounts", body, httpOptions)
                        .pipe(
                            tap((response: any) => {
                                return true;
                            }),
                            catchError(this.handleError)
                        );
    }  

    login(userName, password) {

        const httpOptions = {
            headers: new HttpHeaders({ 'Content-Type': 'application/json' })
        };

        return this.httpClient
                        .post(this.baseUrl + '/auth/login', JSON.stringify({ userName, password }), httpOptions)
                        .pipe(
                            tap((response: any) => {
                                localStorage.setItem('auth_token', response.auth_token);
                                this.loggedIn = true;
                                this._authNavStatusSource.next(true);
                                return true;
                            }),
                            catchError(this.handleError)
                        );
    }
  
    logout() {
        localStorage.removeItem('auth_token');
        this.loggedIn = false;
        this._authNavStatusSource.next(false);
    }
  
    isLoggedIn() {
        return this.loggedIn;
    }
  
    linkedInLogin(accessToken:string) {
        
        const httpOptions = {
            headers: new HttpHeaders({ 'Content-Type': 'application/json' })
        };

        let body = JSON.stringify({ accessToken });  
        
        return this.httpClient
                        .post(this.baseUrl + '/externalauth/linkedin', body, httpOptions)
                        .pipe(
                            tap((response: any) => {
                                localStorage.setItem('auth_token', response.auth_token);
                                this.loggedIn = true;
                                this._authNavStatusSource.next(true);
                                return true;
                            }),
                            catchError(this.handleError)
                        );
    }
}