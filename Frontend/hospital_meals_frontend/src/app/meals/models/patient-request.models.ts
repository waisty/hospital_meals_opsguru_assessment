export enum MealRequestApprovalStatus {
  Pending = 0,
  Analyzing = 1,
  Rejected = 2,
  Accepted = 3,
}

export interface PatientRequestViewModel {
  id: string;
  patientId: string;
  patientName: string;
  recipeId: string;
  requestedDateTime: Date;
  approvalStatus: MealRequestApprovalStatus;
  statusReason: string | null;
  unsafeIngredientId: string | null;
  finalizedDateTime: Date | null;
}

export interface PatientRequestCreateRequest {
  patientId: string;
  recipeId: string;
}

export interface PatientRequestCreateResponse {
  id: string;
  statusString: string;
  statusReason: string;
  unsafeIngredientId: string;
}

export interface SafetyCheckViewModel {
  isSafe: boolean;
  statusReason: string | null;
  unsafeIngredientId: string | null;
}
