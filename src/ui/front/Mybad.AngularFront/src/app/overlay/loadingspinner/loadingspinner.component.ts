import { NgIf } from '@angular/common';
import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-loadingspinner',
  standalone: true,
  imports: [NgIf],
  templateUrl: './loadingspinner.component.html',
  styleUrl: './loadingspinner.component.css'
})
export class LoadingspinnerComponent {
  isLoading = input<boolean>(false);

  show = computed(() => this.isLoading());
}
