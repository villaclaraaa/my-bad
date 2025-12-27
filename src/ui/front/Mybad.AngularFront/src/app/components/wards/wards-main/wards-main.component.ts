import { Component, computed, effect, inject, signal } from '@angular/core';
import { TabsmenuComponent } from "../tabsmenu/tabsmenu.component";
import { NgSwitch, NgSwitchCase, NgIf, NgClass } from '@angular/common';
import { WardmapComponent } from "../wardmap/wardmap.component";
import { FormsModule } from '@angular/forms';
import { EfficiencymapComponent } from "../efficiencymap/efficiencymap.component";
import { WardsService } from '../../../services/wards.service';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { filter, map, switchMap, tap } from 'rxjs';
import { PlayerService } from '../../../services/player.service';
import { ErrorComponent } from '../../../overlay/error/error.component';
import { LoadingspinnerComponent } from '../../../overlay/loadingspinner/loadingspinner.component';

@Component({
  selector: 'app-wards-main',
  standalone: true,
  imports: [TabsmenuComponent,
    NgSwitch, NgSwitchCase, NgIf,
    WardmapComponent,
    FormsModule,
    EfficiencymapComponent,
    ErrorComponent,
    LoadingspinnerComponent, NgClass],
  templateUrl: './wards-main.component.html',
  styleUrl: './wards-main.component.css'
})
export class WardsMainComponent {

  private wardsService = inject(WardsService);
  private playerService = inject(PlayerService);

  constructor() {
    effect(() => console.log("active tab", this.activeTab()));
  };

  avatarUrl: string = '';
  accountName: number | string = 'None';
  searchQuery: string = '';

  accountId = signal<number | null>(null);

  apiErrors = signal<string[]>([]);
  isLoading = signal(false);
  searchAccount() {
    const accountId1 = Number(this.searchQuery);

    if (Number.isNaN(accountId1)) {
      this.accountName = 'not found';
      this.avatarUrl = '';
      return;
    }

    this.isLoading.set(true);
    this.playerService.getBasePlayerInfo(accountId1).subscribe({
      next: (data) => {

        if (data.errors.length > 0) {
          this.apiErrors.set(data.errors);
          this.isLoading.set(false);
          this.activeTab.set('none');
          this.accountName = 'not found';
          this.avatarUrl = '';
          return;
        }
        if (!data.playerInfo) {
          this.accountName = 'not found';
          this.avatarUrl = '';
          this.isLoading.set(false);
          this.activeTab.set('none');
          return;
        }

        this.apiErrors.set([]);
        this.accountName = data.playerInfo.personaName;
        this.avatarUrl = data.playerInfo.avatarMediumUrl;

        this.activeTab.set('map');
        this.accountId.set(accountId1);
        this.isLoading.set(false);
      },
      error: () => {
        this.accountName = 'not found';
        this.avatarUrl = '';
        this.isLoading.set(false);
      }
    });
  }

  // UI state
  activeTab = signal<'map' | 'efficiency' | 'none'>('none');

  // allWards = toSignal(
  //   toObservable(this.accountId).pipe(
  //     filter((id): id is number => id !== null),
  //     switchMap(id =>
  //       this.wardsService.getWardsMap(id).pipe(
  //         map(res => res.observerWards)
  //       )
  //     ),
  //     tap(() => this.isLoading.set(false))
  //   ),
  //   { initialValue: [] }
  // );

  // efficiencyWards = toSignal(
  //   toObservable(this.accountId).pipe(
  //     filter((id): id is number => id !== null),
  //     switchMap(id =>
  //       this.wardsService.getWardsEfficiency(id).pipe(
  //         map(res => res.observerWards)
  //       )
  //     )
  //   ),
  //   { initialValue: [] }
  // );


  /* THE METHOD ONLY FOR DEVELOPMENT
  * WHEN THE APP STARTUP THE ALLWARDS AND EFFWARDS ARE LOADED ONLY ONCE
  * THIS IS TO PREVENT OVER REQUESTING TO ODOTA
  */
  // derived state
  // wardsForCurrentTab = computed(() =>
  //   this.activeTab() === 'map' ? this.allWards() : this.efficiencyWards());


  /*
  *
  * THIS SHOULD BE ON PROD
  * COMMENTING IT BECAUSE WHEN SWITCHING TABS IT WILL ALWAYS PERFORM CALLS TO ODOTA
  * SO WE GET VERY FAST COOLDOWN ON REQUESTS
  * SO METHOD ABOVE IS USED IN DEVELOPMENT
  */
  //   wardsForCurrentTab = toSignal(
  //   toObservable(this.activeTab).pipe(
  //     switchMap(tab => {
  //       if (tab === 'map') {
  //         return this.wardsService.getWardsMap(136996088).pipe(
  //           map(res => res.observerWards)
  //         );
  //       }

  //       return this.wardsService.getWardsEfficiency(136996088).pipe(
  //         map(res =>
  //           res.observerWards.map(w => ({
  //             x: w.x,
  //             y: w.y,
  //             amount: w.amount,
  //           }))
  //         )
  //       );
  //     })
  //   ),
  //   { initialValue: [] }
  // );

}