import { Component } from '@angular/core';
import { WardmapComponent } from "../wardmap/wardmap.component";
import { TabsmenuComponent } from "../tabsmenu/tabsmenu.component";
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-wards-main',
  standalone: true,
  imports: [WardmapComponent, TabsmenuComponent, NgIf],
  templateUrl: './wards-main.component.html',
  styleUrl: './wards-main.component.css'
})
export class WardsMainComponent {

  activeTab: string = 'map';
}
