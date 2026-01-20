import { Component, computed, HostListener, inject, input, OnInit, output, signal } from '@angular/core';
import { NgFor, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { combineLatest, filter, map, shareReplay, startWith, Subject, switchMap, tap } from 'rxjs';
import { WardsService } from '../../../services/wards.service';
import { WardSimpleEfficiency } from '../../../models/wardsModels';
import { ErrorComponent } from '../../../overlay/error/error.component';
import { HeronamespipePipe } from '../../../pipes/heronamespipe.pipe';
type SideFilter = 'all' | 'radiant' | 'dire';

@Component({
  selector: 'app-efficiencymap',
  standalone: true,
  imports: [NgFor, CommonModule, FormsModule, ErrorComponent, HeronamespipePipe],
  templateUrl: './efficiencymap.component.html',
  styleUrl: './efficiencymap.component.css'
})
export class EfficiencymapComponent implements OnInit {
  private wardsService = inject(WardsService);
  
  ngOnInit(): void {
    this.isLoadingOutput.emit(this.isLoadingpage);
  }

  // Loading spinner variables.
  private isLoadingpage: boolean = true;
  isLoadingOutput = output<boolean>();
  
  apiErrors = signal<string[]>([]);
  accountId = input<number>(0);
  
  private refresh$ = new Subject<void>();
  apiResponse = toSignal(
    combineLatest([
      toObservable(this.accountId).pipe(
        filter((id): id is number => id !== null)
      ),
      this.refresh$.pipe(
        map(() => true),          // refresh click
        startWith(false)          // initial load
      )
    ]).pipe(
      tap(() => this.isLoadingOutput.emit(true)),
      switchMap(([id, forceRefresh]) =>
        this.wardsService.getWardsEfficiencyCached(id, forceRefresh).pipe(
          tap(() => this.isLoadingOutput.emit(false))
        )
      )
    ),
    { initialValue: null }
  );

  matchIds = computed(() => {
    const side = this.selectedSide(); // 👈 dependency
    const matches = this.apiResponse()?.includedMatches ?? [];

    const filteredMatches =
      side === 'all'
        ? matches
        : matches.filter(w => w.isRadiantPlayer === (side === 'radiant')); // adjust property name
    return [...filteredMatches].sort((a, b) => b.matchId - a.matchId);
  });

  // Is used as intermediate layer between apiResponse and scaledWards, which are used in html.
  // Does some sorting and filtering.
  wardsList = computed(() => {
    const wards = this.apiResponse()?.observerWards ?? [];
    const side = this.selectedSide(); // 👈 dependency

    const filteredWards =
      side === 'all'
        ? wards
        : wards.filter(w => w.isRadiantSide === (side === 'radiant')); // adjust property name

    return [...filteredWards].sort((a, b) => b.efficiencyScore - a.efficiencyScore);
  });

  /* 
   * Gets color for wards efficiency based on efficiency number.
   * Now we have 4 ranges - [0,0.25], [0.25,0.5], [0.5-0.75], [0.75-1].
   */  
  getEfficiencyClasses(efficiency: number): string {
    const step = Math.min(3, Math.floor(efficiency * 4));
    return this.colors[step];
  }
  private colors = [
    "bg-red-500/50 border border-red-500",
    "bg-orange-500/50 border border-orange-500",
    "bg-yellow-500/50 border border-lime-500",
    "bg-green-500/50 border border-green-500",
  ];

  /*
   * Stuff for showing tooltip what is active ward.
   */
  activeWard = signal<WardSimpleEfficiency | null>(null);
  tooltipPos = signal<{ x: number; y: number } | null>(null);
  toggleWard(w: WardSimpleEfficiency, event: MouseEvent) {
    event.stopPropagation();

    const isSame =
      this.activeWard()?.x === w.x &&
      this.activeWard()?.y === w.y;
    if (isSame) {
      this.closeTooltip();
      return;
    }

    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;

    let x = event.clientX + this.OFFSET;
    let y = event.clientY + this.OFFSET;

    // 🔹 Clamp horizontally
    if (x + this.TOOLTIP_WIDTH > viewportWidth) {
      x = event.clientX - this.TOOLTIP_WIDTH - this.OFFSET;
    }

    // 🔹 Clamp vertically
    if (y + this.TOOLTIP_HEIGHT > viewportHeight) {
      y = viewportHeight - this.TOOLTIP_HEIGHT - this.OFFSET;
    }

    // 🔹 Prevent negative positions
    x = Math.max(this.OFFSET, x);
    y = Math.max(this.OFFSET, y);

    this.activeWard.set(w);
    this.tooltipPos.set({ x, y });
  }

  isActive(w: WardSimpleEfficiency) {
    const active = this.activeWard();
    return active?.x === w.x && active?.y === w.y;
  }

  selectedSide = signal<SideFilter>('all');
  onSideChange(side: SideFilter) {
    this.selectedSide.set(side);
  }


  @HostListener('document:click')
  onDocumentClick() {
    this.closeTooltip();
  }

  closeTooltip() {
    this.activeWard.set(null);
    this.tooltipPos.set(null);
  }

  private readonly TOOLTIP_WIDTH = 160; // px (matches w-40)
  private readonly TOOLTIP_HEIGHT = 110; // approx
  private readonly OFFSET = 12;
  /* End of stuff about tooltip 
  */

  /* WARDS POSITIONS RELATED STUFF 
  * OLD (DOTAMAPCOMPONENT)
  */
  // -----------------------------
  // IMAGE SIZE SIGNALS
  // -----------------------------
  imageWidth = signal(1);
  imageHeight = signal(1);

  // Original image size (static)
  readonly originalWidth = 900;
  readonly originalHeight = 900;

  // -----------------------------
  // MAP CONSTANTS
  // -----------------------------
  readonly mapSize = 900;

  readonly minCoord = 66;
  readonly maxCoord = 192;

  readonly minCoordY = 62;
  readonly maxCoordY = 190;

  // -----------------------------
  // DERIVED SIGNALS
  // -----------------------------
  readonly coordRange = this.maxCoord - this.minCoord;

  readonly coordRangeY = this.maxCoordY - this.minCoordY;

  // -----------------------------
  // DOM REFERENCE
  // -----------------------------
  private mapImageElement?: HTMLImageElement;

  // -----------------------------
  // IMAGE LOAD + RESIZE
  // -----------------------------
  onImageLoad(img: HTMLImageElement): void {
    this.mapImageElement = img;
    this.updateImageSize();
  }

  @HostListener('window:resize')
  onResize(): void {
    this.updateImageSize();
  }

  private updateImageSize(): void {
    if (!this.mapImageElement) return;

    this.imageWidth.set(this.mapImageElement.clientWidth);
    this.imageHeight.set(this.mapImageElement.clientHeight);
  }

  // -----------------------------
  // FINAL SCALED WARDS (COMPUTED)
  // -----------------------------
  scaledWards = computed(() => {
    const iw = this.imageWidth();   // reactive
    const ih = this.imageHeight();  // reactive
    const size = this.circleSize();

    return this.wardsList().map(w => {
      // scale X/Y first
      const baseX = ((w.x - this.minCoord) / this.coordRange) * this.mapSize;
      const baseY = this.mapSize - ((w.y - this.minCoordY) / this.coordRangeY) * this.mapSize;
      return {
        ...w,
        left: (baseX / this.originalWidth) * iw,
        top: (baseY / this.originalHeight) * ih,
        size
      };
    });
  });

  // STUFF related to outer circle scaling with screen size
  readonly referenceImageSize = 600;
  readonly referenceCircleSize = 78;

  readonly circleRatio =
    this.referenceCircleSize / this.referenceImageSize;

  circleSize = computed(() => {
    return Math.min(this.imageWidth(), this.imageHeight()) * this.circleRatio;
  });
}
