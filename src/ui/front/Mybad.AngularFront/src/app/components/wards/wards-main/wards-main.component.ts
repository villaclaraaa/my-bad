import { Component, computed, inject, signal } from '@angular/core';
import { TabsmenuComponent } from "../tabsmenu/tabsmenu.component";
import { NgIf, NgSwitch, NgSwitchCase } from '@angular/common';
import { DotamapComponent } from "../dotamap/dotamap.component";
import { WardmapComponent } from "../wardmap/wardmap.component";
import { FormsModule } from '@angular/forms';
import { EfficiencymapComponent } from "../efficiencymap/efficiencymap.component";
import { WardsService } from '../../../services/wards.service';
import { WardSimple } from '../../../models/wardsModels';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

@Component({
  selector: 'app-wards-main',
  standalone: true,
  imports: [TabsmenuComponent, NgIf, NgSwitch, NgSwitchCase, DotamapComponent, WardmapComponent, FormsModule, EfficiencymapComponent],
  templateUrl: './wards-main.component.html',
  styleUrl: './wards-main.component.css'
})
export class WardsMainComponent {

  private wardsService = inject(WardsService);

  accountName: any;
  searchQuery: any;
  searchAccount() {
    throw new Error('Method not implemented.');
  }
  // UI state
  activeTab = signal<'map' | 'efficiency'>('map');

   allWards = toSignal(
    this.wardsService.getWardsMap(136996088).pipe(
      map(res => res.ObserverWards)
    ),
    { initialValue: [] } // default empty array
  );

  efficiencyWards = signal<WardSimple[]>([
    { X: 122, Y: 122, Amount: 1 }
  ]);

  // derived state
  wardsForCurrentTab = computed(() => 
    this.activeTab() === 'map' ? this.allWards() : this.efficiencyWards());

  // example tab switch
  setTab(tab: 'map' | 'efficiency') {
    this.activeTab.set(tab);
  }
}