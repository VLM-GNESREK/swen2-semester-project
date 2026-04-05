import {TourService} from '../../services/tourService';
import {TourDtoAngular} from '../../models/tourModel';
import {AsyncPipe, CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {Observable, interval, switchMap} from 'rxjs';
import {Component, ChangeDetectorRef} from '@angular/core';

@Component
({
  selector: 'app-tour-dashboard',
  imports: [CommonModule,FormsModule],
  templateUrl: './tour-dashboard.html',
  styleUrl: './tour-dashboard.scss',
})

export class TourDashboard 
{
  //so we can refresh data on the front end immediately while waiting for the "pull"
  toursUIonly: TourDtoAngular[] = [];
  newTour: TourDtoAngular = {id: 0, name: 't', description: '', from: '', to: '', transportType: ''};

  editingTourId: number | null = null; // stores which tour is being edited
  editedTour: TourDtoAngular = {} as TourDtoAngular;


  constructor(
    private tourService: TourService,
    //we need change detector because Angular doesnt see new data when switching maps for whatever reason
    //change detector forces the visual update
    private changeDetectorRef: ChangeDetectorRef
  ) {
  }

  ngOnInit() 
  {
    this.tourService.getTours().subscribe(toursData => 
    {
      this.toursUIonly = toursData;
      this.changeDetectorRef.detectChanges();
    });

    interval(5000).pipe(switchMap(() => this.tourService.getTours())).subscribe(toursData => 
    {
      this.toursUIonly = toursData
      this.changeDetectorRef.detectChanges();
    });
  }

  addTour() 
  {
    const tempTour: TourDtoAngular = 
    {
      id: 0,
      name: this.newTour.name,
      description: this.newTour.description,
      from: this.newTour.from,
      to: this.newTour.to,
      transportType: this.newTour.transportType,
      distance: this.newTour.distance,
      estimatedTime: this.newTour.estimatedTime,
      routeInformation: this.newTour.routeInformation
    };
    this.toursUIonly = [...this.toursUIonly, tempTour];
    this.newTour = 
    {
      id: 0,
      name: 't',
      description: '',
      from: '',
      to: '',
      transportType: '',
      distance: 0,
      estimatedTime: 0,
      routeInformation: ''
    };
    this.tourService.createTour(tempTour).subscribe();
  }

  deleteTour(id: number) 
  {
    this.toursUIonly = this.toursUIonly.filter(t => t.id !== id); // filter out tour we want to delete
    this.tourService.deleteTour(id).subscribe();
  }

  editTour(tour: TourDtoAngular) 
  {
    this.editingTourId = tour.id;
    this.editedTour = { ...tour };
  }

  cancelEdit() {
    this.editingTourId = null;
  }

  saveTour(id: number) 
  {
    this.toursUIonly = this.toursUIonly.map(t => t.id === id ? { ...this.editedTour } : t);
    this.editingTourId = null;

    this.tourService.updateTour(id, this.editedTour).subscribe();
  }
}
