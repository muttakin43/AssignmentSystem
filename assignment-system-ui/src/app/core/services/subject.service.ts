import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Subject, CreateSubjectRequest, UpdateSubjectRequest } from '../models';

@Injectable({
  providedIn: 'root',
})
export class SubjectService {
   private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/subject`;

  getAll(): Observable<Subject[]> {
    return this.http.get<Subject[]>(this.baseUrl);
  }

  getById(id: string): Observable<Subject> {
    return this.http.get<Subject>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateSubjectRequest): Observable<Subject> {
    return this.http.post<Subject>(this.baseUrl, request);
  }

  update(id: string, request: UpdateSubjectRequest): Observable<Subject> {
    return this.http.put<Subject>(`${this.baseUrl}/${id}`, request);
  }

  deactivate(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
