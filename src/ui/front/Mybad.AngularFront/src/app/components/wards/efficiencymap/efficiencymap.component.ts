import { Component, computed, HostListener, inject, input, Input, OnInit, output, signal } from '@angular/core';
import { NgFor, NgStyle, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { combineLatest, filter, map, shareReplay, startWith, Subject, switchMap, tap } from 'rxjs';
import { WardsService } from '../../../services/wards.service';
import { WardSimpleEfficiency } from '../../../models/wardsModels';
import { ErrorComponent } from '../../../overlay/error/error.component';
type SideFilter = 'all' | 'radiant' | 'dire';

@Component({
  selector: 'app-efficiencymap',
  standalone: true,
  imports: [NgFor, CommonModule, FormsModule, ErrorComponent],
  templateUrl: './efficiencymap.component.html',
  styleUrl: './efficiencymap.component.css'
})
export class EfficiencymapComponent implements OnInit {
  ngOnInit(): void {
        this.isLoadingOutput.emit(this.isLoadingpage);
  }
  newMatchId: string = '';
  sortedWards: { id: number, efficiency: number }[] = [
    { id: 1, efficiency: 100 },
    { id: 2, efficiency: 90 },
  ]
  apiErrors = signal<string[]>([]);
  private refresh$ = new Subject<void>();


  removeMatchId(matchId: number) {
    this.isLoadingOutput.emit(true); 
    this.wardsService.removeMatchIdFromParsedMatchesEfficiency(matchId, this.accountId())
    .subscribe({
      next: (res) => {
        console.log('Match removed:', res);
        this.refresh$.next();
      },
      error: (err) => {
        console.error('Failed to remove match:', err);
        this.apiErrors.set(['Failed to remove match:', err]);
      }
    });
  }


  
  accountId = input<number>(0);
  private wardsService = inject(WardsService);

  matchIds = computed(() => this.apiResponse()?.includedMatches);

  private isLoadingpage: boolean = true;

  isLoadingOutput = output<boolean>();

//   apiResponse = toSignal(
//   toObservable(this.accountId).pipe(
//     filter((id): id is number => id !== null),
//     switchMap(id =>
//       this.wardsService.getWardsEfficiencyCached(id).pipe(
//         map(res => res),
//         tap(() => {
//           this.isLoadingOutput.emit(false); 
//         })
//       )
//     )
//   ),
//   { initialValue: null }
// );

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


  
wardsList = computed(() => {
  const wards = this.apiResponse()?.observerWards ?? [];
  return [...wards].sort((a, b) => b.efficiencyScore - a.efficiencyScore);
});


hoveredWard = signal<WardSimpleEfficiency | null>(null);

isHovered(w: WardSimpleEfficiency) {
  const hovered = this.hoveredWard();
  return hovered?.x === w.x && hovered?.y === w.y;
}

private colors = [
  "bg-red-500/50 border border-red-500",    
  "bg-orange-500/50 border border-orange-500", 
  "bg-yellow-500/50 border border-lime-500",   
  "bg-green-500/50 border border-green-500",  
];

 getEfficiencyClasses(efficiency: number): string {
  const step = Math.min(3, Math.floor(efficiency * 4));
  return this.colors[step];
}

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

  // wards = input<WardSimpleMap[]>([]);

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
