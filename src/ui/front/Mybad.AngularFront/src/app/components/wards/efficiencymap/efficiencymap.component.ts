import { Component, input, Input } from '@angular/core';
import { NgFor, NgStyle, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { WardSimple } from '../../../models/wardsModels';


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
   wards = input<WardSimple[]>([]);
 

  // IMPORTANT: set this to 128 or 256 depending on your data
  gridSize = 256;

  dotaMax = 255;

  // Convert OpenDota tile -> percentage for CSS
  getLeft(x: number): string {
    return (x / this.gridSize * 100) + '%'
  }

  getTop(y: number): string {
    // Flip Y because images start at top-left but OD coordinates start at bottom-left
    return (100 - (y / this.gridSize * 100)) + '%';
  }


  mapSize = 900;
  pxPerUnit = this.mapSize / 256;

  offsetX = 68.657;
  offsetY = -65.424;

  scaleX(x: number) {
    return (x - this.offsetX) * this.pxPerUnit;
  }

  scaleY(y: number) {
    return this.mapSize - ((y - this.offsetY) * this.pxPerUnit);
  }


  matchIds: number[] = [1, 2, 3];
}
