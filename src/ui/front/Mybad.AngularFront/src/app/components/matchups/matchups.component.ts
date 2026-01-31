import { Component, inject, signal, computed, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HeroService } from '../../services/hero.service';
import { Hero } from '../../models/hero.interface';
import { HeroMatchup } from '../../models/matchup.interface';
import { LoadingspinnerComponent } from "../../overlay/loadingspinner/loadingspinner.component";
import { skip } from 'rxjs';
import e from 'express';
import { HeronamespipePipe } from '../../pipes/heronamespipe.pipe';

@Component({
  selector: 'app-matchups',
  standalone: true,
  imports: [CommonModule, FormsModule, LoadingspinnerComponent, HeronamespipePipe],
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

  // Hero Pool
  heroPool = signal<Hero[]>([]);
  isPoolEnabled = signal<boolean>(false);
  currentPoolName = signal<string>('');
  isPoolDropdownOpen = signal<boolean>(false);
  isAddingNewPool = signal<boolean>(false);
  newPoolName = '';

  // Picker State
  isPicking = signal<boolean>(false);
  pickingSide = signal<'my' | 'enemy' | 'pool' | null>(null);

  // Results
  bestAlly = signal<HeroMatchup[]>([]);
  bestComputed = signal<HeroMatchup[]>([]);
  bestVersus = signal<HeroMatchup[]>([]);


  isLoadingAlly = signal<boolean>(false);
  isLoadingVersus = signal<boolean>(false);
  isLoadingComputed = signal<boolean>(false);

  constructor() {
    this.heroService.getHeroes().subscribe((data: Hero[]) => {
      this.heroes.set(data);
      this.loadHeroPoolFromLocalStorage();
    });
  }

  filteredGroupedHeroes = computed(() => {
    const text = this.searchText().toLowerCase();
    const all = this.heroes();
  
    let filtered: Hero[] = [];
   if(this.pickingSide() === 'pool') {
     filtered = all.filter(hero => hero.localized_name.toLowerCase().includes(text))
     .filter(hero => !this.heroPool().find(h => h.id === hero.id));
    }
    else{
     filtered = all.filter(hero => hero.localized_name.toLowerCase().includes(text))
       .filter(hero => !this.myTeam().find(h => h.id === hero.id) && !this.enemyTeam().find(h => h.id === hero.id));
   }
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

  openPicker(side: 'my' | 'enemy' | 'pool') {
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
    } else if (side === 'pool') {
      if (!this.heroPool().find(h => h.id === hero.id)) {
        this.heroPool.update(pool => [...pool, hero]);
        this.saveHeroPoolToLocalStorage();
      }
      this.closePicker();
      return;
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
    const poolIds = this.heroPool().map(h => h.id);

    // Convert empty arrays to null for API
    const allyIdsOrNull = allyIds.length > 0 ? allyIds : null;
    const enemyIdsOrNull = enemyIds.length > 0 ? enemyIds : null;
    let poolIdsOrNull = poolIds.length > 0 ? poolIds : null;

    if(!this.isPoolEnabled()) {
      poolIdsOrNull = null;
    }

    console.log(poolIdsOrNull);
    if (allyIds.length === 0 && enemyIds.length === 0) {
      this.bestAlly.set([]);
      this.bestComputed.set([]);
      this.bestVersus.set([]);
      return;
    }

    // 1. Best Ally (only if we have allies)
    if (allyIds.length > 0) {
      if (!skipAllies) {

        if (!this.isLoadingAlly()) {
          this.isLoadingAlly.set(true);
          this.heroService.findBestHeroes(allyIdsOrNull, null, poolIdsOrNull).subscribe(res => {
            this.bestAlly.set(res.matchup.slice(0, 10));
            this.isLoadingAlly.set(false);
          });
        }
      }
    } else {
      this.bestAlly.set([]);
    }

    // 2. Best Versus (only if we have enemies)
    if (enemyIds.length > 0 ) {
      if (!skipEnemies) {
        if (!this.isLoadingVersus()) {
          this.isLoadingVersus.set(true);
          this.heroService.findBestHeroes(null, enemyIdsOrNull, poolIdsOrNull).subscribe(res => {
            this.bestVersus.set(res.matchup.slice(0, 10));
            this.isLoadingVersus.set(false);
          });
        }
      }
    } else {
      this.bestVersus.set([]);
    }

    // 3. Best Computed (Combined)
    if (allyIds.length > 0 || enemyIds.length > 0) {
      if (!this.isLoadingComputed()) {
        this.isLoadingComputed.set(true);
        this.heroService.findBestHeroes(allyIdsOrNull, enemyIdsOrNull, poolIdsOrNull).subscribe(res => {
          this.bestComputed.set(res.matchup.slice(0, 10));
          this.isLoadingComputed.set(false);
        });
      }
    } else {
      this.bestComputed.set([]);
    }
  }

  // Hero Pool Management
  loadHeroPoolFromLocalStorage() {
    const storedData = localStorage.getItem('heroPoolIds');
    if (storedData) {
      try {
        const data = JSON.parse(storedData);
        // Check if it's the new format (object with multiple pools)
        if (typeof data === 'object' && !Array.isArray(data)) {
          const currentPool = this.currentPoolName();
          if (currentPool && data[currentPool]) {
            const poolHeroes = this.heroes().filter(hero => data[currentPool].includes(hero.id));
            this.heroPool.set(poolHeroes);
          } else if (!currentPool && Object.keys(data).length > 0) {
            // If no pool selected but pools exist, select the first one
            const firstPool = Object.keys(data)[0];
            this.currentPoolName.set(firstPool);
            const poolHeroes = this.heroes().filter(hero => data[firstPool].includes(hero.id));
            this.heroPool.set(poolHeroes);
          }
        } else if (Array.isArray(data)) {
          // Legacy format: migrate to new format with a default name
          const poolHeroes = this.heroes().filter(hero => data.includes(hero.id));
          this.heroPool.set(poolHeroes);
          this.currentPoolName.set('My Pool');
          // Save in new format
          this.saveHeroPoolToLocalStorage();
        }
      } catch (e) {
        console.error('Error loading hero pool from local storage', e);
      }
    }
  }

  saveHeroPoolToLocalStorage() {
    if (!this.currentPoolName()) return; // Don't save if no pool selected
    
    const ids = this.heroPool().map(h => h.id);
    const storedData = localStorage.getItem('heroPoolIds');
    let pools: { [key: string]: number[] } = {};
    
    if (storedData) {
      try {
        const data = JSON.parse(storedData);
        if (typeof data === 'object' && !Array.isArray(data)) {
          pools = data;
        }
      } catch (e) {
        console.error('Error parsing stored pools', e);
      }
    }
    
    pools[this.currentPoolName()] = ids;
    localStorage.setItem('heroPoolIds', JSON.stringify(pools));
  }

  removeHeroFromPool(hero: Hero) {
    this.heroPool.update(pool => pool.filter(h => h.id !== hero.id));
    this.saveHeroPoolToLocalStorage();
  }

  changeUsingHeroPool() {
    this.isPoolEnabled.set(!this.isPoolEnabled());
    this.calculateMatchups(false, false);
  }
  
  swapTeams() {
    const tempMyTeam = this.myTeam();
    this.myTeam.set(this.enemyTeam());
    this.enemyTeam.set(tempMyTeam); 
    this.calculateMatchups(false, false);
  }

  // Pool Dropdown Methods
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const dropdown = target.closest('.pool-dropdown-container');
    
    if (!dropdown && this.isPoolDropdownOpen()) {
      this.isPoolDropdownOpen.set(false);
    }
  }

  togglePoolDropdown() {
    this.isPoolDropdownOpen.set(!this.isPoolDropdownOpen());
  }

  getPoolNames(): string[] {
    const storedData = localStorage.getItem('heroPoolIds');
    if (storedData) {
      try {
        const data = JSON.parse(storedData);
        if (typeof data === 'object' && !Array.isArray(data)) {
          return Object.keys(data);
        }
      } catch (e) {
        console.error('Error getting pool names', e);
      }
    }
    return [];
  }

  selectPool(poolName: string) {
    this.currentPoolName.set(poolName);
    this.isPoolDropdownOpen.set(false);
    this.loadHeroPoolFromLocalStorage();
    this.calculateMatchups(false, false);
  }

  openAddPoolDialog() {
    this.isPoolDropdownOpen.set(false);
    this.isAddingNewPool.set(true);
    this.newPoolName = '';
  }

  cancelAddPool() {
    this.isAddingNewPool.set(false);
    this.newPoolName = '';
  }

  confirmAddPool() {
    if (!this.newPoolName.trim()) return;
    
    const poolName = this.newPoolName.trim();
    const storedData = localStorage.getItem('heroPoolIds');
    let pools: { [key: string]: number[] } = {};
    
    if (storedData) {
      try {
        const data = JSON.parse(storedData);
        if (typeof data === 'object' && !Array.isArray(data)) {
          pools = data;
        }
      } catch (e) {
        console.error('Error parsing stored pools', e);
      }
    }
    
    pools[poolName] = [];
    localStorage.setItem('heroPoolIds', JSON.stringify(pools));
    
    this.currentPoolName.set(poolName);
    this.heroPool.set([]);
    this.isAddingNewPool.set(false);
    this.newPoolName = '';
    this.calculateMatchups(false, false);
  }

  deletePool(poolName: string, event: Event) {
    event.stopPropagation(); // Prevent triggering selectPool
    
    const storedData = localStorage.getItem('heroPoolIds');
    if (!storedData) return;
    
    try {
      const data = JSON.parse(storedData);
      if (typeof data === 'object' && !Array.isArray(data)) {
        delete data[poolName];
        localStorage.setItem('heroPoolIds', JSON.stringify(data));
        
        // If deleted pool was the current one, reset
        if (this.currentPoolName() === poolName) {
          const remainingPools = Object.keys(data);
          if (remainingPools.length > 0) {
            // Select first remaining pool
            this.currentPoolName.set(remainingPools[0]);
            this.loadHeroPoolFromLocalStorage();
          } else {
            // No pools left
            this.currentPoolName.set('');
            this.heroPool.set([]);
          }
          this.calculateMatchups(false, false);
        }
      }
    } catch (e) {
      console.error('Error deleting pool', e);
    }
  }
}
 