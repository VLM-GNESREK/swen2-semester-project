import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {CommonModule} from '@angular/common';
import { FormsModule } from '@angular/forms';
import {TourComponent} from './tour-component/tour-component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TourComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})

export class App {
  protected readonly title = signal('TourPlanner.Frontend');
}
