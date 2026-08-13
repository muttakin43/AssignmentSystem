import { Injectable,inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ClassRoom, ClassDetail, CreateClassRequest, UpdateClassRequest } from '../models';

@Injectable({
  providedIn: 'root',
})
export class ClassService {
   private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/class`;

  getAll(): Observable<ClassRoom[]> {
    return this.http.get<ClassRoom[]>(this.baseUrl);
  }

  getById(id: string): Observable<ClassDetail> {
    return this.http.get<ClassDetail>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateClassRequest): Observable<ClassRoom> {
    return this.http.post<ClassRoom>(this.baseUrl, request);
  }

  update(id: string, request: UpdateClassRequest): Observable<ClassRoom> {
    return this.http.put<ClassRoom>(`${this.baseUrl}/${id}`, request);
  }

  deactivate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  linkSubject(classId: string, subjectId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${classId}/subjects/${subjectId}`, {});
  }

  unlinkSubject(classId: string, subjectId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${classId}/subjects/${subjectId}`);
  }
}
