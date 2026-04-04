import { TourDtoAngular } from '../models/tourModel';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root',
})

export class TourService
{
  private apiUrl = 'http://localhost:5239/api/tours';

  constructor(private http: HttpClient) {}

  getTours(): Observable<TourDtoAngular[]>
  {
    return this.http.get<TourDtoAngular[]>(this.apiUrl);
  }

  getTourById(id: number): Observable<TourDtoAngular>
  {
    console.log("IM HERE");
    var test = this.http.get<TourDtoAngular>(`${this.apiUrl}/${id}`);
    console.log(this.http.get<TourDtoAngular>(`${this.apiUrl}/${id}`));
    return test
  }

  createTour(tour: TourDtoAngular)
  {
    return this.http.post(this.apiUrl, tour);
  }

  deleteTour(id: number)
  {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
  updateTour(id: number, tour: TourDtoAngular) {
    return this.http.put(`${this.apiUrl}/${id}`,tour);
  }
}
