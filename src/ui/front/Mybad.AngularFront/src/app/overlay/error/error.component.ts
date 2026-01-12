import { NgFor, NgIf } from '@angular/common';
import { Component, computed, input, Signal } from '@angular/core';

@Component({
  selector: 'app-error',
  standalone: true,
  imports: [NgIf, NgFor],
  templateUrl: './error.component.html',
  styleUrl: './error.component.css'
})
export class ErrorComponent {
  errorMessages = input<string[]>([]);

  hasErrors = computed(() => this.errorMessages().length > 0);
}
