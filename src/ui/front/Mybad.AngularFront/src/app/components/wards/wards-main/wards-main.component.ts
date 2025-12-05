import { Component } from '@angular/core';
import { TabsmenuComponent } from "../tabsmenu/tabsmenu.component";
import { NgIf, NgSwitch, NgSwitchCase } from '@angular/common';
import { DotamapComponent } from "../dotamap/dotamap.component";
import { WardmapComponent } from "../wardmap/wardmap.component";
import { FormsModule } from '@angular/forms';
import { EfficiencymapComponent } from "../efficiencymap/efficiencymap.component";

@Component({
  selector: 'app-wards-main',
  standalone: true,
  imports: [TabsmenuComponent, NgIf, NgSwitch, NgSwitchCase, DotamapComponent, WardmapComponent, FormsModule, EfficiencymapComponent],
  templateUrl: './wards-main.component.html',
  styleUrl: './wards-main.component.css'
})
export class WardsMainComponent {
  accountName: any;
  searchQuery: any;
  searchAccount() {
    throw new Error('Method not implemented.');
  }

  activeTab = 'map';

  allWards: { x: number; y: number }[] = [
    { x: 90, y: 146 },
    { x: 122, y: 122 },
    { x: 113, y: 151 },
    { x: 120, y: 65 },
    { x: 99, y: 88 },
    { x: 94, y: 120 },
    { x: 160, y: 120 },
    { x: 139, y: 91 },
    { x: 150, y: 116 },
  ];    // your full list
  efficiencyWards: { x: number; y: number }[] = [
    { x: 90, y: 146 },
    { x: 122, y: 122 },
    { x: 113, y: 151 },
    { x: 120, y: 65 },
    { x: 99, y: 88 },
    { x: 94, y: 120 },
  ]; // wards highlighted for efficiency tab

  get wardsForCurrentTab() {
    console.log(this.activeTab);
    return this.allWards;
    // return this.activeTab === 'map'
    //   ? this.allWards
    //   : this.efficiencyWards;
  }
}
