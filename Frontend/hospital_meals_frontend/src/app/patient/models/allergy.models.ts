export interface AllergyViewModel {
  id: string;
  name: string;
}

export interface AllergyCreateRequest {
  name: string;
}

export interface AllergyCreateResponse {
  id: string;
}

export interface AllergyUpdateRequest {
  name: string;
}
