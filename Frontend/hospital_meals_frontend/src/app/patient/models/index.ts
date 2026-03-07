export type {
  PatientViewModel,
  PatientWithDietTypeNameViewModel,
  PatientAllergyViewModel,
  PatientClinicalStateViewModel,
  PatientDetailViewModel,
  PatientCreateRequest,
  PatientCreateResponse,
  PatientUpdateRequest,
  PatientAllergiesUpdateRequest,
  PatientClinicalStatesUpdateRequest,
  BatchPatientAllergiesRequest,
  BatchPatientAllergiesResponse,
  PatientAllergiesItemViewModel,
} from './patient.models';

export type {
  AllergyViewModel,
  AllergyCreateRequest,
  AllergyCreateResponse,
  AllergyUpdateRequest,
} from './allergy.models';

export type {
  ClinicalStateViewModel,
  ClinicalStateCreateRequest,
  ClinicalStateCreateResponse,
  ClinicalStateUpdateRequest,
} from './clinical-state.models';

export type {
  DietTypeViewModel,
  DietTypeCreateRequest,
  DietTypeCreateResponse,
  DietTypeUpdateRequest,
} from './diet-type.models';
