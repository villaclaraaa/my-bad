import { AfterViewInit, Component, Input, HostListener } from '@angular/core';
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
 @Input() wards: { x: number; y: number }[] = [
    { x: 90, y: 146 },
    { x: 122, y: 122 },
    { x: 113, y: 151 },
    { x: 120, y: 65 },
    { x: 99, y: 88 },
    { x: 94, y: 120 },
  ]

  // ---------------------------------------------
  // ORIGINAL IMAGE SIZE (must match your real file)
  // ---------------------------------------------
  originalWidth: number = 900;
  originalHeight: number = 900;

  // ---------------------------------------------
  // CURRENT RENDERED IMAGE SIZE
  // (updated automatically on load and on resize)
  // ---------------------------------------------
  imageWidth: number = 1;
  imageHeight: number = 1;

  gridSize = 256;

  // Reference to the image DOM element
  private mapImageElement!: HTMLImageElement;

  constructor() {}

  ngAfterViewInit(): void {
    // nothing needed here yet
  }

  // Called from template: (load)="onImageLoad(mapImg)"
  onImageLoad(img: HTMLImageElement): void {
    this.mapImageElement = img;
    this.updateImageSize();
  }

  // Recalculate when resizing window
  @HostListener('window:resize')
  onResize(): void {
    this.updateImageSize();
  }

  // ---------------------------------------------
  // Reads current rendered image size (responsive)
  // ---------------------------------------------
  updateImageSize(): void {
    if (!this.mapImageElement) return;

    this.imageWidth = this.mapImageElement.clientWidth;
    this.imageHeight = this.mapImageElement.clientHeight;

    // Debug:
    // console.log("Image size:", this.imageWidth, this.imageHeight);
  }

  // getLeft(x: number): string {
  //   return (x / this.gridSize * 100) + '%';
  // }

  // getTop(y: number): string {
  //   // Flip Y because images start at top-left but OD coordinates start at bottom-left
  //   return (100 - (y / this.gridSize * 100)) + '%';
  // }

  getLeft(x: number): number {
    return (x / this.gridSize * 100);
  }

  getTop(y: number): number {
    // Flip Y because images start at top-left but OD coordinates start at bottom-left
    return (100 - (y / this.gridSize * 100));
  }

  dotaMax = 128;
  mapSize = 900;

   minCoord = 66;        // playable area min
  maxCoord = 192;       // playable area max

  
   minCoordY = 62;        // playable area min
  maxCoordY = 190;       // playable area max

  // computed range
  get coordRange() {
    return this.maxCoord - this.minCoord;   // 195
  }

  get coordRangeY() {
    return this.maxCoordY - this.minCoordY;   // 195
  }

  // X axis scales left → right
  scaleX(x: number): number {
    const normalized = (x - this.minCoord) / this.coordRange;
    return normalized * this.mapSize;
  }

  // Y axis must flip (game origin bottom-left, HTML top-left)
  scaleY(y: number): number {
    const normalized = (y - this.minCoordY) / this.coordRangeY;
    return this.mapSize - normalized * this.mapSize;
  }

  // ---------------------------------------------
  // SCALE X/Y BASED ON RESPONSIVE IMAGE SIZE
  // ---------------------------------------------
  getScaledX(x: number): number {
    let zz = this.scaleX(x);
    return (zz / this.originalWidth) * this.imageWidth;
  }

  getScaledY(y: number): number {
    let zz = this.scaleY(y);
    return (zz / this.originalHeight) * this.imageHeight;
  }
}
