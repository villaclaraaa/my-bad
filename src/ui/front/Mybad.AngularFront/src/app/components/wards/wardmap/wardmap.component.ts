import { Component, computed, effect, HostListener, inject, input, Input, OnInit, output, signal } from '@angular/core';
import { NgFor, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { WardSimpleMap } from '../../../models/wardsModels';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { WardsService } from '../../../services/wards.service';
import { filter, map, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-wardmap',
  standalone: true,
  imports: [NgFor, CommonModule, FormsModule],
  templateUrl: './wardmap.component.html',
  styleUrl: './wardmap.component.css'
})
export class WardmapComponent implements OnInit {
  isLoading = output<boolean>();
  isLoadingpage: boolean = true;
  ngOnInit(): void {
    this.isLoading.emit(this.isLoadingpage);
  }

  // wards = input<WardSimpleMap[]>([]);

  accountId = input<number>(136996088);
  private wardsService = inject(WardsService);

  matchIds: number[] = [1, 2, 3];

  apiResponse = toSignal(
    toObservable(this.accountId).pipe(
      filter((id): id is number => id !== null),
      switchMap(id =>
        this.wardsService.getWardMapCached(id).pipe(
          map(res => res),
          tap(() => { this.isLoadingpage = false; this.isLoading.emit(this.isLoadingpage) })
        )
      )
    ),
    { initialValue: null }
  );


  wardsList = computed(() => {
  const wards = this.apiResponse()?.observerWards ?? [];
  return [...wards].sort((a, b) => b.amount - a.amount);
});

hoveredWard = signal<WardSimpleMap | null>(null);

isHovered(w: WardSimpleMap) {
  const hovered = this.hoveredWard();
  return hovered?.x === w.x && hovered?.y === w.y;
}

  /* WARDS POSITIONS STUFF
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

  ngAfterViewInit(): void { }

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
  // CORE SCALING (PURE)
  // -----------------------------
  private scaleX(x: number): number {
    const normalized =
      (x - this.minCoord) / this.coordRange;
    return normalized * this.mapSize;
  }

  private scaleY(y: number): number {
    const normalized =
      (y - this.minCoordY) / this.coordRangeY;
    return this.mapSize - normalized * this.mapSize;
  }

  // -----------------------------
  // FINAL SCALED WARDS (COMPUTED)
  // -----------------------------
  scaledWards = computed(() => {
    const iw = this.imageWidth();   // reactive
    const ih = this.imageHeight();  // reactive
    return this.wardsList().map(w => {
      // scale X/Y first
      const baseX = ((w.x - this.minCoord) / this.coordRange) * this.mapSize;
      const baseY = this.mapSize - ((w.y - this.minCoordY) / this.coordRangeY) * this.mapSize;
      return {
        ...w,
        left: (baseX / this.originalWidth) * iw,
        top: (baseY / this.originalHeight) * ih,
      };
    });
  });

  getDotClassForHeatMap(amount: number): string {
    switch (true) {
      case amount < 2:
        return `${this.heatmapOverlay[0]} z-10`

      case amount < 3:
        return `${this.heatmapOverlay[1]} z-10`

      case amount < 4:
        return `${this.heatmapOverlay[2]} z-15`

      case amount < 5:
        return `${this.heatmapOverlay[3]} z-20`

      case amount < 6:
        return `${this.heatmapOverlay[4]} z-25 w-3.5 h-3.5`

      case amount < 8:
        return `${this.heatmapOverlay[5]} z-30 w-3.5 h-3.5`

      case amount < 10:
        return `${this.heatmapOverlay[6]} z-35 w-3.5 h-3.5`

      case amount < 15:
        return `${this.heatmapOverlay[7]} z-40 w-4 h-4`

      case amount < 20:
        return `${this.heatmapOverlay[8]} z-45 w-4 h-4`

      default:
        return `${this.heatmapOverlay[9]} z-50 w-5 h-5`
    }
  }

  private heatmapOverlay = [
    'bg-blue-500/50',
    'bg-blue-500/60',
    'bg-cyan-500/70',
    'bg-emerald-500/70',
    'bg-lime-500/80',
    'bg-yellow-500/80',
    'bg-orange-500/90',
    'bg-red-500/50',
    'bg-red-600/60',
    'bg-red-700/70',
  ];
}
