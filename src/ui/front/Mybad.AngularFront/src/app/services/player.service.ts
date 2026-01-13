import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Player } from '../models/playerinfo';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {

  private http = inject(HttpClient);

  private readonly defaultHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
  });

  private dataUrl = 'assets/json/heroes.json';
  private apiUrl = '/api/players';

  getBasePlayerInfo(accId: number): Observable<Player> {
    const url = `${this.apiUrl}/baseinfo?accountId=${accId}`;
    return this.http.get<Player>(url, { headers: this.defaultHeaders });
   }
}
