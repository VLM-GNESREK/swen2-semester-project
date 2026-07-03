import { Component } from '@angular/core';
import {FormsModule, NgForm} from '@angular/forms';
import { AuthService } from '../../services/auth-service';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {

  loginData = {
    username: '',
    password: ''
  };

  constructor(private auth: AuthService) {}

  login(form: NgForm) {
    if (form.invalid) return;

    this.auth.login(this.loginData).subscribe({
      next: (res: any) => {
        console.log('Login successful', res);

        // store token (adjust if your API returns different shape)
        if (res.token) {
          this.auth.saveToken(res.token);
          console.log(res.token);
          alert('Login successful');
        }

        form.resetForm();
      },
      error: (err) => {
        console.error('Login failed', err);
      }
    });
  }
}
