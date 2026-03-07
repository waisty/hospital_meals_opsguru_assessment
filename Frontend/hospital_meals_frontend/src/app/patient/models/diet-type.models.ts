export interface DietTypeViewModel {
  id: string;
  name: string;
}

export interface DietTypeCreateRequest {
  name: string;
}

export interface DietTypeCreateResponse {
  id: string;
}

export interface DietTypeUpdateRequest {
  name: string;
}
