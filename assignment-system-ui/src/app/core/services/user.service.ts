import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserDTO, UserQuery, CreateUserRequest, UpdateUserRequest, ChangePasswordRequest, PageResult } from '../models';
@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/user`;

  getPaged(query: UserQuery): Observable<PageResult<UserDTO>> {
    let params = new HttpParams();
    if (query.page) params = params.set('page', query.page);
    if (query.pageSize) params = params.set('pageSize', query.pageSize);
    if (query.role) params = params.set('role', query.role);
    if (query.classId) params = params.set('classId', query.classId);
    if (query.search) params = params.set('search', query.search);

    return this.http.get<PageResult<UserDTO>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<UserDTO> {
    return this.http.get<UserDTO>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateUserRequest): Observable<UserDTO> {
    return this.http.post<UserDTO>(this.baseUrl, request);
  }

  update(id: string, request: UpdateUserRequest): Observable<UserDTO> {
    return this.http.put<UserDTO>(`${this.baseUrl}/${id}`, request);
  }

  setActive(id: string, isActive: boolean): Observable<UserDTO> {
    return this.http.patch<UserDTO>(`${this.baseUrl}/${id}/active`, isActive);
  }

  changePassword(id: string, request: ChangePasswordRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/change-password`, request);
  }
}
