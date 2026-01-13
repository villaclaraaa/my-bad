import { NgIf, NgClass } from '@angular/common';
import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-loadingspinner',
  standalone: true,
  imports: [NgIf, NgClass],
  templateUrl: './loadingspinner.component.html',
  styleUrl: './loadingspinner.component.css'
})
export class LoadingspinnerComponent {
  isLoading = input<boolean>(false);
  cssClasses = input<string[]>([]);
  show = computed(() => this.isLoading());
}
