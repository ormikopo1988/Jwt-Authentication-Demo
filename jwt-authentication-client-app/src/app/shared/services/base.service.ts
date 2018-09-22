import { throwError } from 'rxjs';

export abstract class BaseService {  
    
    constructor() { }

    protected handleError(httpErrorResponse: any) {
        
        var applicationError = httpErrorResponse.headers.get('Application-Error');

        // either applicationError in header or model error in body
        if (applicationError) {
            return throwError(applicationError);
        }

        var modelStateErrors: string = '';
        var serverError = httpErrorResponse.error;

        if (!serverError.type) {
            for (var key in serverError) {
                if (serverError[key]) {
                    modelStateErrors += serverError[key] + '\n';
                }
            }
        }

        modelStateErrors = modelStateErrors = '' ? null : modelStateErrors;
        
        return throwError(modelStateErrors || 'Server error');
        
    }
}