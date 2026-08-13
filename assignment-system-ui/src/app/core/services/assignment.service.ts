import { Injectable ,inject} from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AssignmentDto, AssignmentQuery, CreateAssignmentRequest, UpdateAssignmentRequest, PageResult, AssignmentStatus } from '../models';
@Injectable({
  providedIn: 'root',
})
export class AssignmentService {
   private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/Assignment`;

  getPaged(query: AssignmentQuery): Observable<PageResult<AssignmentDto>> {
    let params = new HttpParams();
    if (query.page) params = params.set('page', query.page);
    if (query.pageSize) params = params.set('pageSize', query.pageSize);
    if (query.classId) params = params.set('classId', query.classId);
    if (query.subjectId) params = params.set('subjectId', query.subjectId);
    if (query.status) params = params.set('status', query.status);
    if (query.search) params = params.set('search', query.search);

    return this.http.get<PageResult<AssignmentDto>>(this.baseUrl, { params });
  }

  getById(id: string): Observable<AssignmentDto> {
    return this.http.get<AssignmentDto>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateAssignmentRequest): Observable<AssignmentDto> {
    return this.http.post<AssignmentDto>(this.baseUrl, request);
  }

  update(id: string, request: UpdateAssignmentRequest): Observable<AssignmentDto> {
    return this.http.put<AssignmentDto>(`${this.baseUrl}/${id}`, request);
  }

  publish(id: string): Observable<AssignmentDto> {
    return this.http.patch<AssignmentDto>(`${this.baseUrl}/${id}/publish`, {});
  }

  close(id: string): Observable<AssignmentDto> {
    return this.http.patch<AssignmentDto>(`${this.baseUrl}/${id}/close`, {});
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
