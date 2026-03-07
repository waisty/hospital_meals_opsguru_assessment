export interface UserAuthRequest {
  username: string;
  password: string;
}

export interface UserAuthResponse {
  authToken: string;
  admin: boolean;
  patientAdmin: boolean;
  mealsAdmin: boolean;
  mealsUser: boolean;
  kitchenUser: boolean;
}
