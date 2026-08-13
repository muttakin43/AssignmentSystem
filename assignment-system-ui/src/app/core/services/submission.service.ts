import { Injectable ,inject} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SubmissionDto, GradeSubmissionRequest } from '../models';
@Injectable({
  providedIn: 'root',
})
export class SubmissionService {
    private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

 getForAssignment(assignmentId: string) {
  return this.http.get<SubmissionDto[]>(
    `${this.baseUrl}/Submission/assignments/${assignmentId}/submissions`
  );
}

getMine(): Observable<SubmissionDto[]> {
  return this.http.get<SubmissionDto[]>(
    `${this.baseUrl}/Submission/submissions/mine`
  );
}

  getById(id: string): Observable<SubmissionDto> {
    return this.http.get<SubmissionDto>(`${this.baseUrl}/submissions/${id}`);
  }

  create(
  assignmentId: string,
  textAnswer: string | null,
  file: File | null
): Observable<SubmissionDto> {

  const formData = new FormData();

  if (textAnswer)
    formData.append('textAnswer', textAnswer);

  if (file)
    formData.append('file', file);

  return this.http.post<SubmissionDto>(
    `${this.baseUrl}/Submission/assignments/${assignmentId}/submissions`,
    formData
  );
}

  update(id: string, textAnswer: string | null, file: File | null): Observable<SubmissionDto> {
    const formData = new FormData();
    if (textAnswer) formData.append('textAnswer', textAnswer);
    if (file) formData.append('file', file);
    return this.http.put<SubmissionDto>(
  `${this.baseUrl}/Submission/submissions/${id}`,
  formData
);
  }

grade(id: string, payload: any) {
  return this.http.put(
    `${this.baseUrl}/Submission/submissions/${id}/grade`,
    payload
  );
}

 downloadFile(id: string) {
  return this.http.get(
    `${this.baseUrl}/Submission/submissions/${id}/file`,
    {
      responseType: 'blob'
    }
  );
}
}
