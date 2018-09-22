import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { HomeDetails } from '../models/home.details.interface'; 
import { ConfigService } from '../../shared/utils/config.service';

import {BaseService} from '../../shared/services/base.service';

import { Observable } from 'rxjs'; 
import { tap, catchError } from 'rxjs/operators';

@Injectable()

export class DashboardService extends BaseService {

    baseUrl: string = ''; 

    constructor(private httpClient: HttpClient, private configService: ConfigService) {
        super();
        this.baseUrl = configService.getApiURI();
    }

    getHomeDetails(): Observable<HomeDetails> {
        
        let authToken = localStorage.getItem('auth_token');

        let bearerTokenHeader = `Bearer ${authToken}`;
        
        const httpOptions = {
            headers: new HttpHeaders({ 
                'Content-Type': 'application/json',
                'Authorization': bearerTokenHeader
            })
        };

        return this.httpClient
                        .get(this.baseUrl + "/dashboard/home", httpOptions)
                        .pipe(
                            tap((response: any) => response),
                            catchError(this.handleError)
                        );
    }  
}