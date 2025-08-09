import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const isApiRequest = req.url.startsWith(environment.apiURL);

  if (isApiRequest) {
    const modifiedReq = req.clone({
      withCredentials: true
    });
    return next(modifiedReq);
  }

  return next(req);
};
