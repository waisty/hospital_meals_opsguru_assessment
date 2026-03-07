export interface PatientViewModel {
  id: string;
  firstName: string;
  middleName: string;
  lastName: string;
  mobileNumber: string;
  dietTypeId: string;
  notes: string | null;
}

/** Patient with diet type name (from join); returned by list patients endpoint. */
export interface PatientWithDietTypeNameViewModel extends PatientViewModel {
  dietTypeName: string;
}

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
  firstName: string;
  middleName: string;
  lastName: string;
  mobileNumber: string;
  dietTypeId: string;
  notes: string | null;
  allergies: PatientAllergyViewModel[];
  clinicalStates: PatientClinicalStateViewModel[];
}

export interface PatientCreateRequest {
  firstName: string;
  middleName: string;
  lastName: string;
  mobileNumber: string;
  dietTypeId: string;
  notes?: string | null;
}

export interface PatientCreateResponse {
  id: string;
}

export interface PatientUpdateRequest {
  firstName: string;
  middleName: string;
  lastName: string;
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

export interface BatchPatientAllergiesRequest {
  patientIds: string[];
}

export interface PatientAllergiesItemViewModel {
  patientId: string;
  allergyNames: string[];
}

export interface BatchPatientAllergiesResponse {
  items: PatientAllergiesItemViewModel[];
}

export interface BatchPatientClinicalStatesRequest {
  patientIds: string[];
}

export interface PatientClinicalStatesItemViewModel {
  patientId: string;
  clinicalStateNames: string[];
}

export interface BatchPatientClinicalStatesResponse {
  items: PatientClinicalStatesItemViewModel[];
}
