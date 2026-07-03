import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {UserDataModel} from '../models/userDataModel';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5239/api/auth';

  constructor(private http: HttpClient) {}

  login(credentials: UserDataModel): Observable<UserDataModel> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials);
  }

  register(credentials: UserDataModel) {
    return this.http.post(`${this.apiUrl}/register`, credentials);
  }
  saveToken(token: string) {
    localStorage.setItem('token', token);
  }
  getToken() {
    return localStorage.getItem('token');
  }
  logout() {
    localStorage.removeItem('token');
  }
  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
