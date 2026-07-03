import {Routes} from '@angular/router';
import {TourDashboard} from './components/tour-dashboard/tour-dashboard';
import {TourDetailsComponent} from './components/tour-details/tour-details';
import {TourLogsComponent} from './components/tour-logs/tour-logs';
import {Login} from './components/login/login';
import {Register} from './components/register/register';
import {AuthGuard} from './guards/auth-guard';

export const routes: Routes = [
  {path: '', redirectTo: '/login', pathMatch: 'full'},
  {path: 'tours', component: TourDashboard, canActivate: [AuthGuard]},
  {path: 'tours/:id', component: TourDetailsComponent,canActivate: [AuthGuard]},
  {path: 'tours/:id/logs', component: TourLogsComponent,canActivate: [AuthGuard]},
  {path: 'login', component: Login},
  {path: 'register', component: Register},
];
