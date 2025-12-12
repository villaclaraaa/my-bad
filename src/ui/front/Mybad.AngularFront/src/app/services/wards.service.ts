import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WardsMapApiResponse } from '../models/wardsModels';

@Injectable({
  providedIn: 'root'
})
export class WardsService {

  private basePath: string = `api/wards/`;

  constructor(private http: HttpClient) { }

  private headers1: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });


  getWardsMap(accId: number) : Observable<WardsMapApiResponse> {
    const url = `${this.basePath}/map`;
    return this.http.get<WardsMapApiResponse>(url);
  }

}
