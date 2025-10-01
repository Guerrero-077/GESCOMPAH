export interface FinanceSelectModels {
  id: number;
  key: string;
  value: string;
  active: boolean;
}

export interface FinanceCreateModels {
  id: number;
  key: string;
  value: string;
  effectiveFrom: Date;
  effectiveTo: Date;
  active: boolean;
}

export interface FinanceUpdateModels {
  id: number;
  key: string;
  value: string;
  effectiveFrom: Date;
  effectiveTo: Date;
  active: boolean;
}
