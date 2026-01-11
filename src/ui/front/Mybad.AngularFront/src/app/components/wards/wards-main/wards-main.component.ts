import { Component, effect, inject, signal } from '@angular/core';
import { TabsmenuComponent } from "../tabsmenu/tabsmenu.component";
import { NgSwitch, NgSwitchCase, NgIf, NgClass } from '@angular/common';
import { WardmapComponent } from "../wardmap/wardmap.component";
import { FormsModule } from '@angular/forms';
import { EfficiencymapComponent } from "../efficiencymap/efficiencymap.component";
import { WardsService } from '../../../services/wards.service';
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

  private playerService = inject(PlayerService);

  constructor() {
    effect(() => console.log("active tab", this.activeTab()));
  };

  avatarUrl: string = '';
  accountName = signal<string | null>(null);
  searchQuery: string = '';
  accountLinkODota: string = "https://www.opendota.com/players/";
  accountId = signal<number | null>(null);

  apiErrors = signal<string[]>([]);
  isLoading = signal(false);
  searchAccount() {
    const accountId1 = Number(this.searchQuery);

    if (Number.isNaN(accountId1)) {
      this.accountName.set('account not found');
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
          this.accountName.set('account not found');

          this.avatarUrl = '';
          return;
        }
        if (!data.playerInfo) {
          this.accountName.set('account not found');

          this.avatarUrl = '';
          this.isLoading.set(false);
          this.activeTab.set('none');
          this.apiErrors.set(['api did not respond.']);
          return;
        }

        this.apiErrors.set([]);
        this.accountName.set(data.playerInfo.personaName);
        this.avatarUrl = data.playerInfo.avatarMediumUrl;
        this.activeTab.set('map');
        this.accountId.set(accountId1);
        this.accountLinkODota += `${this.accountId()}`;
        this.isLoading.set(false);
      },
      error: () => {
        this.accountName.set('account not found');

        this.avatarUrl = '';
        this.isLoading.set(false);
      }
    });
  }

  // UI state
  activeTab = signal<'map' | 'efficiency' | 'none'>('none');
}