import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HeroService} from '../../services/hero.service';
import { Hero } from '../../models/hero.interface';
import { HeroMatchup } from '../../models/matchup.interface';

@Component({
  selector: 'app-matchups',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
    if (side === 'my') {
      if (this.myTeam().length < 5 && !this.myTeam().find(h => h.id === hero.id)) {
        this.myTeam.update(team => [...team, hero]);
      }
    } else if (side === 'enemy') {
      if (this.enemyTeam().length < 5 && !this.enemyTeam().find(h => h.id === hero.id)) {
        this.enemyTeam.update(team => [...team, hero]);
      }
    }
    this.closePicker();
    this.calculateMatchups();
  }

  removeHero(hero: Hero, side: 'my' | 'enemy') {
    if (side === 'my') {
      this.myTeam.update(team => team.filter(h => h.id !== hero.id));
    } else {
      this.enemyTeam.update(team => team.filter(h => h.id !== hero.id));
    }
    this.calculateMatchups();
  }

  calculateMatchups() {
    const allyIds = this.myTeam().map(h => h.id);
    const enemyIds = this.enemyTeam().map(h => h.id);

    if (allyIds.length === 0 && enemyIds.length === 0) {
        this.bestAlly.set([]);
        this.bestComputed.set([]);
        this.bestVersus.set([]);
        return;
    }

    // 1. Best Ally (only if we have allies)
    if (allyIds.length > 0) {
        this.heroService.findBestHeroes(allyIds, null).subscribe(res => {
            this.bestAlly.set(res.matchup.slice(0, 10));
        });
    } else {
        this.bestAlly.set([]);
    }

    // 2. Best Versus (only if we have enemies)
    if (enemyIds.length > 0) {
        this.heroService.findBestHeroes(null, enemyIds).subscribe(res => {
            this.bestVersus.set(res.matchup.slice(0, 10));
        });
    } else {
        this.bestVersus.set([]);
    }

    // 3. Best Computed (Combined)
    if (allyIds.length > 0 || enemyIds.length > 0) {
        this.heroService.findBestHeroes(allyIds, enemyIds).subscribe(res => {
            this.bestComputed.set(res.matchup.slice(0, 10));
        });
    } else {
        this.bestComputed.set([]);
    }
  }
}
