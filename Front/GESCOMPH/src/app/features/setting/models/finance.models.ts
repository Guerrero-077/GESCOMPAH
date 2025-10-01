export interface FinanceSelectModels {
  id: number;
  key: string;
  value: string;
  effectiveFrom?: string | Date;
  effectiveTo?: string | Date | null;
  active: boolean;
}

export interface FinanceCreateModels {
  id: number;
  key: string;
  value: string;
  effectiveFrom: string | Date;
  effectiveTo: string | Date | null;
  active: boolean;
}

export interface FinanceUpdateModels {
  id: number;
  key: string;
  value: string;
  effectiveFrom: string | Date;
  effectiveTo: string | Date | null;
  active: boolean;
}
