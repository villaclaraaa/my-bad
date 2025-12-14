import { AfterViewInit, Component, Input, HostListener, input, effect, computed, signal } from '@angular/core';
import { WardSimple } from '../../../models/wardsModels';
import { NgStyle, NgForOf } from '@angular/common';

@Component({
  selector: 'app-dotamap',
  standalone: true,
  imports: [NgStyle, NgForOf],
  templateUrl: './dotamap.component.html',
  styleUrl: './dotamap.component.css'
})
export class DotamapComponent implements AfterViewInit {
  //  wards = input<WardSimple[]>([
  //   {X: 80, Y: 80, Amount: 10}
  //  ]);
 
  //  constructor() {
  //   // Log whenever the input changes
  //   effect(() => {
  //     console.log('Received wards from parent:', this.wards());
  //   });
  // }

  // // ---------------------------------------------
  // // ORIGINAL IMAGE SIZE (must match your real file)
  // // ---------------------------------------------
  // originalWidth: number = 900;
  // originalHeight: number = 900;

  // // ---------------------------------------------
  // // CURRENT RENDERED IMAGE SIZE
  // // (updated automatically on load and on resize)
  // // ---------------------------------------------
  // imageWidth: number = 1;
  // imageHeight: number = 1;

  // gridSize = 256;

  // // Reference to the image DOM element
  // private mapImageElement!: HTMLImageElement;


  // ngAfterViewInit(): void {
  //   // nothing needed here yet
  // }

  // // Called from template: (load)="onImageLoad(mapImg)"
  // onImageLoad(img: HTMLImageElement): void {
  //   this.mapImageElement = img;
  //   this.updateImageSize();
  // }

  // // Recalculate when resizing window
  // @HostListener('window:resize')
  // onResize(): void {
  //   this.updateImageSize();
  // }

  // // ---------------------------------------------
  // // Reads current rendered image size (responsive)
  // // ---------------------------------------------
  // updateImageSize(): void {
  //   if (!this.mapImageElement) return;

  //   this.imageWidth = this.mapImageElement.clientWidth;
  //   this.imageHeight = this.mapImageElement.clientHeight;

  //   // Debug:
  //   // console.log("Image size:", this.imageWidth, this.imageHeight);
  // }

  // dotaMax = 128;
  // mapSize = 900;

  //  minCoord = 66;        // playable area min
  // maxCoord = 192;       // playable area max

  
  //  minCoordY = 62;        // playable area min
  // maxCoordY = 190;       // playable area max

  // // computed range
  // get coordRange() {
  //   return this.maxCoord - this.minCoord;   // 195
  // }

  // get coordRangeY() {
  //   return this.maxCoordY - this.minCoordY;   // 195
  // }

  // // X axis scales left → right
  // scaleX(x: number): number {
  //   const normalized = (x - this.minCoord) / this.coordRange;
  //   return normalized * this.mapSize;
  // }

  // // Y axis must flip (game origin bottom-left, HTML top-left)
  // scaleY(y: number): number {
  //   const normalized = (y - this.minCoordY) / this.coordRangeY;
  //   return this.mapSize - normalized * this.mapSize;
  // }

  // // ---------------------------------------------
  // // SCALE X/Y BASED ON RESPONSIVE IMAGE SIZE
  // // ---------------------------------------------
  // getScaledX(x: number): number {
  //   let zz = this.scaleX(x);
  //   return (zz / this.originalWidth) * this.imageWidth;
  // }

  // getScaledY(y: number): number {
  //   let zz = this.scaleY(y);
  //   return (zz / this.originalHeight) * this.imageHeight;
  // }

  wards = input<WardSimple[]>([
    { x: 80, y: 80, amount: 10 },
  ]);

  constructor() {
    effect(() => {
      console.log('Received wards from parent:', this.wards());
  console.log('scaledWards:', this.scaledWards());

    });
  }

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

  ngAfterViewInit(): void {}

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
  return this.wards().map(w => {
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

}
