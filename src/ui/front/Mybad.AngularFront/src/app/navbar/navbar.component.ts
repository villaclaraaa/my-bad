import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
// ⭐ Core Authentication State ⭐
  isLoggedIn: boolean = true; // Set to 'true' to demonstrate logged-in view initially
  username: string = 'DireLord420'; 
  showDropdown: boolean = false; 

  constructor() {
    // Logic to check authentication status goes here
  }

  // Toggles the desktop dropdown menu
  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  // Placeholder for logout functionality
  logout(): void {
    this.isLoggedIn = false;
    this.showDropdown = false;
    // In a real application, you would clear the session and navigate to the home page
  }
}
