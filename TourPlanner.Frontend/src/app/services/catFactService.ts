import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import { catFactModel } from '../models/catFactModel';

@Injectable
({
  providedIn: 'root',
})

export class CatFactService
{
  private apiUrl = 'http://localhost:5239/api/cat';

  constructor(private http: HttpClient) {}

  getCat(): Observable<catFactModel[]>
  {
    return this.http.get<catFactModel[]>(`${this.apiUrl}`);
  }

}
