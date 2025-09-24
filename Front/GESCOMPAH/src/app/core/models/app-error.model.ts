export interface AppError {
  type: 'Validation' | 'Unauthorized' | 'Forbidden' | 'NotFound' | 'Unexpected';
  message: string;
  details?: any;
}