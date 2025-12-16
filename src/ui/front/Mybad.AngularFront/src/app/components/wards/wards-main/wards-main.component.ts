import { Component, computed, effect, inject, signal } from '@angular/core';
import { TabsmenuComponent } from "../tabsmenu/tabsmenu.component";
import { NgIf, NgSwitch, NgSwitchCase, NgSwitchDefault } from '@angular/common';
import { DotamapComponent } from "../dotamap/dotamap.component";
import { WardmapComponent } from "../wardmap/wardmap.component";
import { FormsModule } from '@angular/forms';
import { EfficiencymapComponent } from "../efficiencymap/efficiencymap.component";
import { WardsService } from '../../../services/wards.service';
import { WardSimpleMap } from '../../../models/wardsModels';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { map, tap } from 'rxjs';

@Component({
  selector: 'app-wards-main',
  standalone: true,
  imports: [TabsmenuComponent, NgIf, NgSwitch, NgSwitchCase, NgSwitchDefault, DotamapComponent, WardmapComponent, FormsModule, EfficiencymapComponent],
  templateUrl: './wards-main.component.html',
  styleUrl: './wards-main.component.css'
})
export class WardsMainComponent {

  private wardsService = inject(WardsService);

  constructor(){
    effect(() => console.log("active tab", this.activeTab()));
};

  accountName: number | string = 'None';
  searchQuery: any;
  searchAccount() {
    throw new Error('Method not implemented.');
  }
  // UI state
  activeTab = signal<'map' | 'efficiency'>('map');

  allWards = toSignal(
    this.wardsService.getWardsMap(136996088).pipe(
      tap(res => console.log('API response:', res.observerWards)),
      map(res => {
        const wards = res.observerWards;
        console.log('Extracted wards:', wards);       // log extracted wards
        return wards;
      })),
    { initialValue: [] } // default empty array
  );

  efficiencyWards = toSignal(
    this.wardsService.getWardsEfficiency(136996088).pipe(
      tap(res => console.log('API response:', res.observerWards)),
      map(res => {
        const wards = res.observerWards;
        console.log('Extracted wards:', wards);       // log extracted wards
        return wards;
      })),
    { initialValue: [] } // default empty array
  );


  /* THE METHOD ONLY FOR DEVELOPMENT
  * WHEN THE APP STARTUP THE ALLWARDS AND EFFWARDS ARE LOADED ONLY ONCE
  * THIS IS TO PREVENT OVER REQUESTING TO ODOTA
  */
  // derived state
  wardsForCurrentTab = computed(() =>
    this.activeTab() === 'map' ? this.allWards() : this.efficiencyWards());


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