import { Component, Input } from '@angular/core';
import { NgFor, NgStyle, CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'

@Component({
  selector: 'app-wardmap',
  standalone: true,
  imports: [NgFor, NgStyle, CommonModule, FormsModule],
  templateUrl: './wardmap.component.html',
  styleUrl: './wardmap.component.css'
})
export class WardmapComponent {

  @Input() wards: { x: number; y: number }[] = [
    { x: 90, y: 146 },
    { x: 122, y: 122 },
    { x: 113, y: 151 },
  ];

}
