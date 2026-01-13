import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HeroService} from '../../services/hero.service';
import { Hero } from '../../models/hero.interface';
import { HeroMatchup } from '../../models/matchup.interface';
import { LoadingspinnerComponent } from "../../overlay/loadingspinner/loadingspinner.component";
import { skip } from 'rxjs';

@Component({
  selector: 'app-matchups',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingspinnerComponent],
  templateUrl: './matchups.component.html',
  styleUrls: ['./matchups.component.css']
})
export class MatchupsComponent {
  private heroService = inject(HeroService); 
  
  heroes = signal<Hero[]>([]);
  searchText = signal<string>('');

  // Teams
  myTeam = signal<Hero[]>([]);
  enemyTeam = signal<Hero[]>([]);

  // Picker State
  isPicking = signal<boolean>(false);
  pickingSide = signal<'my' | 'enemy' | null>(null);

  // Results
  bestAlly = signal<HeroMatchup[]>([]);
  bestComputed = signal<HeroMatchup[]>([]);
  bestVersus = signal<HeroMatchup[]>([]);


  isLoadingAlly = signal<boolean>(false);
  isLoadingVersus = signal<boolean>(false);
  isLoadingComputed = signal<boolean>(false);

  constructor(){
    this.heroService.getHeroes().subscribe((data: Hero[]) => {
      this.heroes.set(data);
    });
  }

  filteredGroupedHeroes = computed(() => {
    const text = this.searchText().toLowerCase();
    const all = this.heroes();

    const filtered = all.filter(hero => hero.localized_name.toLowerCase().includes(text));
    
    return {
      str: filtered.filter(h => h.primary_attr === 'str'),
      agi: filtered.filter(h => h.primary_attr === 'agi'),
      int: filtered.filter(h => h.primary_attr === 'int'),
      uni: filtered.filter(h => h.primary_attr === 'all')
    };
  });

  getHeroImg(url: string): string {
    return `https://cdn.cloudflare.steamstatic.com${url}`;
  }

  getHeroImgById(heroId: number): string {
    const hero = this.heroes().find(h => h.id === heroId);
    return hero ? this.getHeroImg(hero.img) : '';
  }

  openPicker(side: 'my' | 'enemy') {
    this.pickingSide.set(side);
    this.isPicking.set(true);
    this.searchText.set('');
  }

  closePicker() {
    this.isPicking.set(false);
    this.pickingSide.set(null);
  }

  selectHero(hero: Hero) {
    const side = this.pickingSide();
    let skipAllies: boolean = false;
    if (side === 'my') {
      if (this.myTeam().length < 5 && !this.myTeam().find(h => h.id === hero.id)) {
        this.myTeam.update(team => [...team, hero]);
      }
    } else if (side === 'enemy') {
      if (this.enemyTeam().length < 5 && !this.enemyTeam().find(h => h.id === hero.id)) {
        this.enemyTeam.update(team => [...team, hero]);
        skipAllies = true;
      }
    }
    this.closePicker();
    this.calculateMatchups(skipAllies, !skipAllies);
  }

  removeHero(hero: Hero, side: 'my' | 'enemy') {
    let skipAllies = false;
    if (side === 'my') {
      this.myTeam.update(team => team.filter(h => h.id !== hero.id));
    } else {
      this.enemyTeam.update(team => team.filter(h => h.id !== hero.id));
      skipAllies = true;
    }
    this.calculateMatchups(skipAllies, !skipAllies);
  }

  calculateMatchups(skipAllies: boolean, skipEnemies: boolean) {
    const allyIds = this.myTeam().map(h => h.id);
    const enemyIds = this.enemyTeam().map(h => h.id);

    // Convert empty arrays to null for API
    const allyIdsOrNull = allyIds.length > 0 ? allyIds : null;
    const enemyIdsOrNull = enemyIds.length > 0 ? enemyIds : null;

    if (allyIds.length === 0 && enemyIds.length === 0) {
        this.bestAlly.set([]);
        this.bestComputed.set([]);
        this.bestVersus.set([]);
        return;
    }

    // 1. Best Ally (only if we have allies)
    if (allyIds.length > 0) {
        if (!skipAllies)
        {

          this.isLoadingAlly.set(true);
          this.heroService.findBestHeroes(allyIdsOrNull, null).subscribe(res => {
            this.bestAlly.set(res.matchup.slice(0, 10));
            this.isLoadingAlly.set(false);
          });
        }
    } else {
        this.bestAlly.set([]);
    }

    // 2. Best Versus (only if we have enemies)
    if (enemyIds.length > 0) {
        if (!skipEnemies)
        {

          this.isLoadingVersus.set(true);
          this.heroService.findBestHeroes(null, enemyIdsOrNull).subscribe(res => {
            this.bestVersus.set(res.matchup.slice(0, 10));
            this.isLoadingVersus.set(false);
          });
        }
    } else {
        this.bestVersus.set([]);
    }

    // 3. Best Computed (Combined)
    if (allyIds.length > 0 || enemyIds.length > 0) {
        this.isLoadingComputed.set(true);
        this.heroService.findBestHeroes(allyIdsOrNull, enemyIdsOrNull).subscribe(res => {
            this.bestComputed.set(res.matchup.slice(0, 10));
            this.isLoadingComputed.set(false);
        });
    } else {
        this.bestComputed.set([]);
    }
  }
}
