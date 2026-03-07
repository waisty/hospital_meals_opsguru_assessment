export enum TrayState {
  Pending = 0,
  PreparationStarted = 1,
  AccuracyValidated = 2,
  EnRoute = 3,
  Delivered = 4,
  Retrieved = 5,
}

/** List item from Kitchen API (GET /api/v1/trays). State values match TrayState enum. */
export interface TrayViewModel {
  id: string;
  patientMealRequestId: string;
  patientId: string;
  patientName: string;
  recipeName: string;
  /** Current state as integer (see TrayState enum). */
  state: number;
  receivedDateTime: string;
  lastUpdateDateTime: string | null;
  stateName: string;
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
