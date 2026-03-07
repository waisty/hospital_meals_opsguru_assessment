export enum TrayState {
  Pending = 0,
  PreparationStarted = 1,
  AccuracyValidated = 2,
  EnRoute = 3,
  Delivered = 4,
  Retrieved = 5,
}

export interface TrayViewModel {
  id: string;
  patientMealRequestId: string;
  patientId: string;
  patientName: string;
  recipeName: string;
  state: TrayState;
  receivedDateTime: Date;
  lastUpdateDateTime: Date | null;
  ingredients: TrayIngredientItem[];
}

export interface TrayIngredientItem {
  ingredientName: string;
  qty: number;
  unit: string | null;
}

export interface AdvanceTrayStateRequest {
  trayId: string;
  fromState: number;
}

export interface AdvanceTrayStateResponse {
  success: boolean;
}
