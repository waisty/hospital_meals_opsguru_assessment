export interface ClinicalStateViewModel {
  id: string;
  name: string;
}

export interface ClinicalStateCreateRequest {
  name: string;
}

export interface ClinicalStateCreateResponse {
  id: string;
}

export interface ClinicalStateUpdateRequest {
  name: string;
}
