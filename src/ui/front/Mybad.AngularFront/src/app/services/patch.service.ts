import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({providedIn: 'root'})
export class PatchService {
  private http = inject(HttpClient);
  private apiUrl = '/api/matchups/patches';

  getAllPatchNames(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrl);
  }
}
