import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WardsMapApiResponse } from '../models/wardsModels';

@Injectable({
  providedIn: 'root'
})
export class WardsService {
  // Modern Angular 18: inject HttpClient instead of constructor injection
  private http = inject(HttpClient);

  // Base path can also be provided via environment.ts or injection token
  private readonly basePath = 'https://localhost:7012/api/wards';

  // Default headers (can be extended per request)
  private readonly defaultHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
  });

  /**
   * Fetch ward map for a given account ID
   * @param accId - account ID
   * @returns Observable of WardsMapApiResponse
   */
  getWardsMap(accId: number): Observable<WardsMapApiResponse> {
    const url = `${this.basePath}/map?accId=${accId}`;
    return this.http.get<WardsMapApiResponse>(url, {
      headers: this.defaultHeaders
    });
  }
}