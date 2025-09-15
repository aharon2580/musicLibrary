import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerData = {
    name: '',
    email: '',
    password: '',
    confirmPassword: ''
  };

  registerError: string | null = null;
  registerSuccess: string | null = null;

  constructor() {}

  register() {
    // Reset messages
    this.registerError = null;
    this.registerSuccess = null;

    // Basic validation
    if (!this.registerData.name || !this.registerData.email || !this.registerData.password) {
      this.registerError = 'All fields are required';
      return;
    }

    if (this.registerData.password !== this.registerData.confirmPassword) {
      this.registerError = 'Passwords do not match';
      return;
    }

    // Temporary debug
    console.log('Register data:', this.registerData);

    // Call your AuthService to actually register user
    this.registerSuccess = 'Registration successful!';
    this.registerData = { name: '', email: '', password: '', confirmPassword: '' };
  }
}
