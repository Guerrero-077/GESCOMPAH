import { HttpErrorResponse } from '@angular/common/http';
import { AppError } from '../../models/app-error.model';

export function mapHttpErrorToAppError(error: HttpErrorResponse): AppError {
  let message = 'Ocurrió un error inesperado';
  let type: AppError['type'] = 'Unexpected';

  if (error.status === 0) {
    type = 'Network';
    message = 'No hay conexión con el servidor. Verifica tu red.';
  } else if (error.status === 401) {
    type = 'Unauthorized';
    message = 'No autorizado. Debes iniciar sesión.';
  } else if (error.status === 403) {
    type = 'Forbidden';
    message = 'No tienes permisos para esta acción.';
  } else if (error.status === 404) {
    type = 'NotFound';
    message = 'Recurso no encontrado.';
  }
  // Validation ProblemDetails (RFC 7807)
  else if (
    (error.status === 400 || error.status === 422) &&
    error.error?.errors
  ) {
    type = 'Validation';
    // Asumimos formato ProblemDetails (RFC 7807)
    const firstKey = Object.keys(error.error.errors)[0];
    const firstMsg = error.error.errors[firstKey][0];
    message = firstMsg || 'Error de validación';
  }
  // Business/unprocessable
  else if (error.status === 422) {
    type = 'Business';
    message =
      error.error?.detail || error.error?.title || 'Operación no válida.';
  } else if (error.status === 409) {
    type = 'Conflict';
    message = error.error?.detail || 'Conflicto al procesar la solicitud.';
  } else if (error.status === 429) {
    type = 'RateLimit';
    message =
      error.error?.detail || 'Demasiadas solicitudes. Intenta más tarde.';
  } else if (typeof error.error === 'string') {
    message = error.error;
  } else if (error.error?.detail) {
    message = error.error.detail;
  }

  return { type, message, details: error.error };
}
