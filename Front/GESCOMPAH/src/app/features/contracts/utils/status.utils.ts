const STATUS_LABEL = {
  PAID: 'Pagado',
  PENDING: 'Pendiente',
  OVERDUE: 'Vencido',
} as const;

type StatusKey = keyof typeof STATUS_LABEL;

function isStatusKey(s: string): s is StatusKey {
  return Object.prototype.hasOwnProperty.call(STATUS_LABEL, s);
}

export function getStatusText(status: string): string {
  return isStatusKey(status) ? STATUS_LABEL[status] : status;
}
