import {Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {TourDtoAngular} from '../models/tourModel';
@Injectable({
  providedIn: 'root',
})

export class TourService {
  private apiUrl = 'http://localhost:5239/api/tours';

  constructor(private http: HttpClient) {
  }

  getTours(): Observable<TourDtoAngular[]> {
    return this.http.get<TourDtoAngular[]>(this.apiUrl);
  }

  createTour(tour: TourDtoAngular) {
    return this.http.post(this.apiUrl, tour);
  }

  deleteTour(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
