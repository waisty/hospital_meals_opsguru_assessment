// export interface PatientViewModel {
//   id: string;
//   name: string;
//   mobileNumber: string;
//   dietTypeId: string;
//   notes: string | null;
// }

export interface PatientAllergyViewModel {
  allergyId: string;
  allergyName: string;
}

export interface PatientClinicalStateViewModel {
  clinicalStateId: string;
  clinicalStateName: string;
}

export interface PatientDetailViewModel {
  id: string;
  name: string;
  mobileNumber: string;
  dietTypeId: string;
  notes: string | null;
  allergies: PatientAllergyViewModel[];
  clinicalStates: PatientClinicalStateViewModel[];
}

export interface PatientCreateRequest {
  name: string;
  mobileNumber: string;
  dietTypeId: string;
  notes?: string | null;
}

export interface PatientCreateResponse {
  id: string;
}

export interface PatientUpdateRequest {
  name: string;
  mobileNumber: string;
  dietTypeId: string;
  notes?: string | null;
}

export interface PatientAllergiesUpdateRequest {
  allergyIds: string[];
}

export interface PatientClinicalStatesUpdateRequest {
  clinicalStateIds: string[];
}
