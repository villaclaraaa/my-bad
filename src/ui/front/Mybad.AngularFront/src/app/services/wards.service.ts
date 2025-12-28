import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { WardsEffApiResponse, WardsMapApiResponse } from '../models/wardsModels';

@Injectable({
  providedIn: 'root'
})
export class WardsService {
  // Modern Angular 18: inject HttpClient instead of constructor injection
  private http = inject(HttpClient);

  private readonly basePath = 'http://localhost:5138/api/wards';

  private readonly defaultHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
  });

  /**
   * Fetch ward map for a given account ID
   * @param accId - account ID
   * @returns Observable of WardsMapApiResponse
   */
  getWardsMap(accId: number): Observable<WardsMapApiResponse> {
    const url = `${this.basePath}/map?accountId=${accId}`;
    return this.http.get<WardsMapApiResponse>(url, {
      headers: this.defaultHeaders
    });
  }

  /**
   * Fetch ward map for a given account ID
   * @param accId - account ID
   * @returns Observable of WardsEffApiResponse
   */
  getWardsEfficiency(accId: number): Observable<WardsEffApiResponse> {
    const url = `${this.basePath}/efficiency?accountId=${accId}`;
    return this.http.get<WardsEffApiResponse>(url, {
      headers: this.defaultHeaders
    });
  }

  private efficiencyCache = new Map<number, Observable<WardsEffApiResponse>>();

  getWardsEfficiencyCached(accountId: number): Observable<WardsEffApiResponse> {
    if (!this.efficiencyCache.has(accountId)) {
      const request$ = this.getWardsEfficiency(accountId)
        .pipe(shareReplay(1));

      this.efficiencyCache.set(accountId, request$);
    }

    return this.efficiencyCache.get(accountId)!;
  }

  private wardmapCache = new Map<number, Observable<WardsMapApiResponse>>();

  getWardMapCached(accountId: number): Observable<WardsMapApiResponse> {
    if (!this.wardmapCache.has(accountId)) {
      const request$ = this.getWardsMap(accountId)
        .pipe(shareReplay(1));

      this.wardmapCache.set(accountId, request$);
    }

    return this.wardmapCache.get(accountId)!;
  }

  removeMatchIdFromParsedMatchesEfficiency(matchId: number, accountId: number) : Observable<any> {
    const url = `${this.basePath}/efficiency/match?matchId=${matchId}&accountId=${accountId}`;
    return this.http.delete(url, {
      headers: this.defaultHeaders
    });
  }
}