import { PageQuery } from "./paged-result.model";
import { UserRole } from "./enums";


export interface UserDTO {
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
  classId: string | null;
  className: string | null;
  isActive: boolean;
  createdAtUtc: string;
}

export interface UserQuery extends PageQuery {
  role?: UserRole;
  classId?: string;
  search?: string;
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  password: string;
  role: UserRole;
  classId: string | null;
}

export interface UpdateUserRequest {
  fullName: string;
  email: string;
  role: UserRole;
  classId: string | null;
}

export interface ChangePasswordRequest {
  newPassword: string;
}