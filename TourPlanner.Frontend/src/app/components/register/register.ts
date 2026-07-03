import { Component } from '@angular/core';
import {FormsModule, NgForm} from '@angular/forms';
import { AuthService } from '../../services/auth-service';
import { UserDataModel } from '../../models/userDataModel';
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {

  registerData: UserDataModel = {
    username: '',
    password: ''
  };

  constructor(private auth: AuthService) {}

  register(form: NgForm) {
    if (form.invalid) return;

    this.auth.register(this.registerData).subscribe({
      next: (res) => {
        console.log('User registered successfully', res);
        form.resetForm();
      },
      error: (err) => {
        console.error('Registration failed', err);
      }
    });
  }
}
