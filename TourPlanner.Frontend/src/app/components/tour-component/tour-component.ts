import { TourService } from '../../services/tourService';
import { TourDtoAngular } from '../../models/tourModel';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {Component} from '@angular/core';

// CommonModule for @For and stuff
// FormsModoule for NgModel
@Component({
  selector: 'app-tour-component',
  imports: [CommonModule,FormsModule],
  templateUrl: './tour-component.html',
  styleUrl: './tour-component.scss',
})

export class TourComponent 
{
  tours: TourDtoAngular[] = [];
  newTour: TourDtoAngular = { id: 0, name: '', description: '', from: '', to: '', transportType: ''};

  constructor(private tourService: TourService) {}

  ngOnInit() 
  {
    this.loadTours();
  }

  loadTours() 
  {
    this.tourService.getTours().subscribe(data => this.tours = data);
  }

  addTour() 
  {
    const tempTour : TourDtoAngular = {id:0, name: this.newTour.name, description: this.newTour.description, from: this.newTour.from, to: this.newTour.to, transportType: this.newTour.transportType, distance: this.newTour.distance, estimatedTime: this.newTour.estimatedTime, routeInformation: this.newTour.routeInformation};
    console.log(tempTour);
    this.newTour = { id: 0, name: 't', description: '', from: '', to: '', transportType: '', distance: 0, estimatedTime: 0, routeInformation: '' };
    this.tourService.createTour(tempTour).subscribe(() => this.loadTours());
  }

  deleteTour(id: number) {
    //this.tourService.deleteTour(id).subscribe(() => this.loadTours());
  }
}
