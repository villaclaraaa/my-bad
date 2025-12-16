import { Component, computed, HostListener, input, Input, signal } from '@angular/core';
import { NgFor, NgStyle, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { WardSimpleMap } from '../../../models/wardsModels';


@Component({
  selector: 'app-efficiencymap',
  standalone: true,
  imports: [NgFor, NgStyle, CommonModule, FormsModule],
  templateUrl: './efficiencymap.component.html',
  styleUrl: './efficiencymap.component.css'
})
export class EfficiencymapComponent {
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


  matchIds: number[] = [1,2,3];

   wards = input<WardSimpleMap[]>([]);


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
