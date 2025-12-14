import { Component, effect, input, Input } from '@angular/core';
import { NgFor, NgStyle, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'
import { WardSimple } from '../../../models/wardsModels';

@Component({
  selector: 'app-wardmap',
  standalone: true,
  imports: [NgFor, NgStyle, CommonModule, FormsModule],
  templateUrl: './wardmap.component.html',
  styleUrl: './wardmap.component.css'
})
export class WardmapComponent {

  wards = input<WardSimple[]>([]);

  constructor() {
      // Log whenever the input changes
      effect(() => {
        console.log('WARDMAPCOMPOENNT wards from parent:', this.wards());
      });
    }

}
