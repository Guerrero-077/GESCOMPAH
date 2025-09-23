import { HttpInterceptorFn } from '@angular/common/http';

export const ngrokCredentialsInterceptor: HttpInterceptorFn = (req, next) => {
  const isNgrok = req.url.includes('ngrok-free.app') || req.url.includes('ngrok.app');

  return next(req.clone({
    withCredentials: true,
    setHeaders: isNgrok ? { 'ngrok-skip-browser-warning': 'true' } : {}
  }));
};
