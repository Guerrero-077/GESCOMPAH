export interface AppError {
  type:
    | 'Validation'
    | 'Business'
    | 'Unauthorized'
    | 'Forbidden'
    | 'NotFound'
    | 'Conflict'
    | 'RateLimit'
    | 'Network'
    | 'Unexpected';
  message: string;
  details?: any;
}
