import { Component, computed, HostListener, inject, input, Input, OnInit, output, signal } from '@angular/core';
import { NgFor, NgStyle, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { filter, map, shareReplay, switchMap, tap } from 'rxjs';
import { WardsService } from '../../../services/wards.service';


@Component({
  selector: 'app-efficiencymap',
  standalone: true,
  imports: [NgFor, CommonModule, FormsModule],
  templateUrl: './efficiencymap.component.html',
  styleUrl: './efficiencymap.component.css'
})
export class EfficiencymapComponent implements OnInit {
  ngOnInit(): void {
        this.isLoading.emit(this.isLoadingpage);
  }
  newMatchId: string = '';
  sortedWards: { id: number, efficiency: number }[] = [
    { id: 1, efficiency: 100 },
    { id: 2, efficiency: 90 },
  ]
  addMatchId() {
    throw new Error('Method not implemented.');
  }
  removeMatchId(_t15: number) {
    throw new Error('Method not implemented.');
  }

  accountId = input<number>(136996088);
  private wardsService = inject(WardsService);

  matchIds: number[] = [1, 2, 3];

  private isLoadingpage: boolean = true;

  isLoading = output<boolean>();

  apiResponse = toSignal(
  toObservable(this.accountId).pipe(
    filter((id): id is number => id !== null),
    switchMap(id =>
      this.wardsService.getWardsEfficiencyCached(id).pipe(
        map(res => res),
        tap(() => {this.isLoadingpage = false; this.isLoading.emit(this.isLoadingpage)})
      )
    )
  ),
  { initialValue: null }
);
    
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

    return this.apiResponse()?.observerWards.map(w => {
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
