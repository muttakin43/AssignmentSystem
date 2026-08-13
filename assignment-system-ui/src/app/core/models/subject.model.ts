export interface Subject {
  id: string;
  name: string;
  code: string | null;
  isActive: boolean;
}

export interface CreateSubjectRequest {
  name: string;
  code: string | null;
}

export interface UpdateSubjectRequest {
  name: string;
  code: string | null;
  isActive: boolean;
}