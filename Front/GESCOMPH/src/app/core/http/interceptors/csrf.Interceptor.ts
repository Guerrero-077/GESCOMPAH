import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

function getCookie(name: string): string | null {
  const m = document.cookie.match(new RegExp('(?:^|; )' + name + '=([^;]*)'));
  return m ? decodeURIComponent(m[1]) : null;
}

export const csrfInterceptor: HttpInterceptorFn = (req, next) => {
  const isApiRequest = req.url.startsWith(environment.apiURL);

  if (isApiRequest) {
    const csrfCookie = getCookie('XSRF-TOKEN');
    req = req.clone({
      withCredentials: true,
      setHeaders: csrfCookie ? { 'X-XSRF-TOKEN': csrfCookie } : {}
    });
  }

  return next(req);
};
