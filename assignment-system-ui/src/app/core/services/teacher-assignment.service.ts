import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TeacherAssignment, CreateTeacherAssignmentRequest } from '../models';

@Injectable({
  providedIn: 'root',
})
export class TeacherAssignmentService{
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/TeacherAssignment`;

  getAll(): Observable<TeacherAssignment[]> {
    return this.http.get<TeacherAssignment[]>(this.baseUrl);
  }

  create(request: CreateTeacherAssignmentRequest): Observable<TeacherAssignment> {
    return this.http.post<TeacherAssignment>(this.baseUrl, request);
  }

  setActive(id: string, isActive: boolean): Observable<TeacherAssignment> {
    return this.http.patch<TeacherAssignment>(`${this.baseUrl}/${id}/active`, isActive);
  }
}
