import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Hero } from '../models/hero.interface';
import { HeroMatchupResponse } from '../models/matchup.interface';

@Injectable({providedIn: 'root'})
export class HeroService {
 private http = inject(HttpClient);

 private dataUrl = 'assets/json/heroes.json';
 private apiUrl = '/api/matchups/find';

 getHeroes(): Observable<Hero[]> {
    return this.http.get<any[]>(this.dataUrl).pipe(
      map(heroes => heroes.map(hero => ({
        id: hero.id || hero.Id,
        localized_name: hero.localized_name,
        primary_attr: hero.primary_attr,
        img: hero.img || hero.Img,
        icon: hero.icon || hero.Icon
      } as Hero)))
    );
 }

 findBestHeroes(allyIds: number[] | null, enemyIds: number[] | null, heroesInPool?: number[] | null, patchStr?: string): Observable<HeroMatchupResponse> {
    return this.http.post<HeroMatchupResponse>(this.apiUrl, { allyIds, enemyIds, heroesInPool, patchStr });
 }
}
