import { Component, effect, inject, signal } from '@angular/core';
import { NgSwitch, NgSwitchCase, NgIf, NgClass, NgFor } from '@angular/common';
import { WardmapComponent } from "../wardmap/wardmap.component";
import { FormsModule } from '@angular/forms';
import { EfficiencymapComponent } from "../efficiencymap/efficiencymap.component";
import { PlayerService } from '../../../services/player.service';
import { ErrorComponent } from '../../../overlay/error/error.component';
import { LoadingspinnerComponent } from '../../../overlay/loadingspinner/loadingspinner.component';
type TabKey = 'map' | 'efficiency' | 'none';


@Component({
  selector: 'app-wards-main',
  standalone: true,
  imports: [NgSwitch, NgSwitchCase, NgIf, NgFor,
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

  /*
   * Base account info section 
   */
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

  /*
   * Tabs section
   */
  activeTab = signal<'map' | 'efficiency' | 'none'>('none');

  tabs = signal<{ key: TabKey; label: string }[]>([
    { key: 'map', label: 'Map' },
    { key: 'efficiency', label: 'Efficiency' }]);

  selectTab(tab: TabKey) {
    this.activeTab.set(tab);
  }
}