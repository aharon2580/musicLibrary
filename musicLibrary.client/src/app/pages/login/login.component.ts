// import { Component } from '@angular/core';
// import { AuthService } from '../../services/auths.service';

// @Component({
//   selector: 'app-login',
//   templateUrl: './login.component.html',
//   styleUrls: ['./login.component.scss']
// })
// export class LoginComponent {
//   constructor(private authService: AuthService) {}

//   login() {
//     this.authService.login({ email: 'test@test.com', password: '1234' })
//       .subscribe({
//         next: res => console.log('Login success', res),
//         error: err => console.error('Login failed', err)
//       });
//   }
// }
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginData = {
    email: '',
    password: ''
  };

  loginError: string | null = null;

  constructor() {}

  login() {
    // Temporary debug log
    console.log('Login data:', this.loginData);

    // Example validation
    if (!this.loginData.email || !this.loginData.password) {
      this.loginError = 'Please enter email and password';
      return;
    }

    this.loginError = null;
    // Call your AuthService here for actual login
  }
}
