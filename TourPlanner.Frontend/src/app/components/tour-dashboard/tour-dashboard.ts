import {TourService} from '../services/tourService';
import {TourDtoAngular} from '../models/tourModel';
import {AsyncPipe, CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {Observable, interval, switchMap} from 'rxjs';
import {Component, ChangeDetectorRef} from '@angular/core';

// CommonModule for @For and stuff
// FormsModoule for NgModel
@Component({
  selector: 'app-tour-dashboard',
  imports: [CommonModule,FormsModule],
  templateUrl: './tour-dashboard.html',
  styleUrl: './tour-dashboard.scss',
})

export class TourComponent {
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

  ngOnInit() {
    //fetch data once before interval
    this.tourService.getTours().subscribe(toursData => {
      this.toursUIonly = toursData;
      this.changeDetectorRef.detectChanges();
    });

    //fetch from API every 5 sec
    interval(5000).pipe(
      switchMap(() => this.tourService.getTours())
    ).subscribe(toursData => {
      this.toursUIonly = toursData

      this.changeDetectorRef.detectChanges();
    });
  }

  addTour() {
    const tempTour: TourDtoAngular = {
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
    console.log(tempTour);
    this.toursUIonly = [...this.toursUIonly, tempTour];
    this.newTour = {
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

  deleteTour(id: number) {
    //we filter out the everything that is not our deleted ID
    this.toursUIonly = this.toursUIonly.filter(t => t.id !== id);

    this.tourService.deleteTour(id).subscribe();

  }

  editTour(tour: TourDtoAngular) {
    this.editingTourId = tour.id;
    // make a copy so edits are local until saved
    this.editedTour = { ...tour };
  }

  cancelEdit() {
    this.editingTourId = null;
  }

  saveTour(id: number) {
    // Update local UI immediately
    this.toursUIonly = this.toursUIonly.map(t =>
      t.id === id ? { ...this.editedTour } : t
    );

    this.editingTourId = null;

    // Send update to backend
    this.tourService.updateTour(id, this.editedTour).subscribe();
  }
}
