import { Routes } from '@angular/router';
import { TourDashboard } from './components/tour-dashboard/tour-dashboard';
import { TourDetailsComponent } from './components/tour-details/tour-details';
import { TourLogs } from './components/tour-logs/tour-logs';

export const routes: Routes = [
  { path: '', redirectTo: '/tours', pathMatch: 'full' },
  { path: 'tours', component: TourDashboard },
  { path: 'tours/:id', component: TourDetailsComponent },
  { path: 'tours/:id/logs', component: TourLogs }
];