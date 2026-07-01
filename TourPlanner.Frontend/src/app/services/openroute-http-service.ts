import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Coords, ToFromCoords} from '../models/openroute-coords';
@Injectable({
  providedIn: 'root',
})
export class OpenrouteHttpService {
  private apiUrl = 'http://localhost:5239/api/openroute';
  constructor(private http: HttpClient) {}

  getToSearchResult(toString: string): Observable<Coords[]>
  {
    return this.http.get<Coords[]>(`${this.apiUrl}/to/${toString}`);
  }

  getFromSearchResult(fromString: string):Observable<Coords[]>
  {
    return this.http.get<Coords[]>(`${this.apiUrl}/from/${fromString}`);
  }

}
