import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-tabsmenu',
  standalone: true,
  imports: [NgFor],
  templateUrl: './tabsmenu.component.html',
  styleUrl: './tabsmenu.component.css'
})
export class TabsmenuComponent {
@Input() tabs: { key: string; label: string }[] = [];
@Input() activeTab: string = 'map';

  selectTab(tab: string) {
    this.activeTab = tab;
    this.activeTabChange.emit(tab); // tell parent

  }

  @Output() activeTabChange = new EventEmitter<string>(); // notify parent

}
