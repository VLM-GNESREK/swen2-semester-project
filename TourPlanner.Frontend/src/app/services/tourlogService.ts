import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import { TourLog } from '../models/tourlogModel';

@Injectable
({
  providedIn: 'root',
})

export class TourLogService 
{
  private apiUrl = 'http://localhost:5239/api/tours';

  constructor(private http: HttpClient) {}

  getTourLogsByTourId(tourId: number): Observable<TourLog[]> 
  {
    return this.http.get<TourLog[]>(`${this.apiUrl}/${tourId}/logs`);
  }

  createTourLog(tourId: number, tourLog: TourLog) 
  {
    return this.http.post(`${this.apiUrl}/${tourId}/logs`, tourLog);
  }

  updateTourLog(tourId: number, tourLogId: number, tourLog: TourLog) 
  {
    return this.http.put(`${this.apiUrl}/${tourId}/logs/${tourLogId}`, tourLog);
  }

  deleteTourLog(tourId: number, tourLogId: number) 
  {
    return this.http.delete(`${this.apiUrl}/${tourId}/logs/${tourLogId}`);
  }
}