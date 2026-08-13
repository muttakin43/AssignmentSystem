import { Injectable,inject} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppSettingDto,CreateAppSettingRequest, UpdateAppSettingRequest } from '../models';
@Injectable({
  providedIn: 'root',
})
export class SettingsService {
   private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/AppSettings`;

  getAll(): Observable<AppSettingDto[]> {
    return this.http.get<AppSettingDto[]>(this.baseUrl);
  }

  create(request: CreateAppSettingRequest): Observable<AppSettingDto> {
    return this.http.post<AppSettingDto>(this.baseUrl, request);
  }

  update(key: string, request: UpdateAppSettingRequest): Observable<AppSettingDto> {
    return this.http.put<AppSettingDto>(`${this.baseUrl}/${key}`, request);
  }

  delete(key: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${key}`);
  }
}
