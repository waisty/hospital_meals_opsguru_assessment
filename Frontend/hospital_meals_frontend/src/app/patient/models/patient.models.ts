export interface PatientViewModel {
  id: string;
  name: string;
  mobileNumber: string;
  dietTypeId: string;
  notes: string | null;
}

export interface PatientDetailViewModel {
  id: string;
  name: string;
  mobileNumber: string;
  dietTypeId: string;
  notes: string | null;
  allergyIds: string[];
  clinicalStateIds: string[];
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
