export interface AppSettingDto {
  id: string;
  key: string;
  value: string;
  description: string | null;
}

export interface CreateAppSettingRequest {
  key: string;
  value: string;
  description: string | null;
}

export interface UpdateAppSettingRequest {
  value: string;
  description: string | null;
}