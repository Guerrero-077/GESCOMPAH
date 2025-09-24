import { HttpErrorResponse } from '@angular/common/http';
import { AppError } from '../../models/app-error.model';

export function mapHttpErrorToAppError(error: HttpErrorResponse): AppError {
  let message = 'Ocurrió un error inesperado';
  let type: AppError['type'] = 'Unexpected';

  if (error.status === 401) {
    type = 'Unauthorized';
    message = 'No autorizado. Debes iniciar sesión.';
  }
  else if (error.status === 403) {
    type = 'Forbidden';
    message = 'No tienes permisos para esta acción.';
  }
  else if (error.status === 404) {
    type = 'NotFound';
    message = 'Recurso no encontrado.';
  }
  else if (error.status === 400 && error.error?.errors) {
    type = 'Validation';
    // Asumimos formato ProblemDetails (RFC 7807)
    const firstKey = Object.keys(error.error.errors)[0];
    const firstMsg = error.error.errors[firstKey][0];
    message = firstMsg || 'Error de validación';
  }
  else if (typeof error.error === 'string') {
    message = error.error;
  }
  else if (error.error?.detail) {
    message = error.error.detail;
  }

  return { type, message, details: error.error };
}
