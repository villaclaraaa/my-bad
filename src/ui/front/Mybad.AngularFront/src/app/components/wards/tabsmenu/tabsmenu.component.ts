import { Component, EventEmitter, input, Input, output, Output } from '@angular/core';
import { NgFor } from '@angular/common';
type TabKey = 'map' | 'efficiency' | 'none';

@Component({
  selector: 'app-tabsmenu',
  standalone: true,
  imports: [NgFor],
  templateUrl: './tabsmenu.component.html',
  styleUrl: './tabsmenu.component.css'
})
export class TabsmenuComponent {
tabs = input<{ key: TabKey; label: string }[]>([]);
  activeTab = input<TabKey>('none');

  activeTabChange = output<TabKey>();

  selectTab(tab: TabKey) {
    this.activeTabChange.emit(tab);
  }
}
