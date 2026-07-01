import { TourService } from '../../services/tourService';
import {OpenrouteHttpService} from '../../services/openroute-http-service';
import { TourDtoAngular } from '../../models/tourModel';
import {RouterLink} from '@angular/router';
import {AsyncPipe, CommonModule} from '@angular/common';
import {FormsModule, NgForm} from '@angular/forms';
import {Observable, interval, switchMap,debounceTime,distinctUntilChanged, Subject} from 'rxjs';
import {Component, ChangeDetectorRef} from '@angular/core';
import {OpenrouteManager} from '../../services/openroute/openroute-manager';
import {Coords,OpenRoute,ToFromCoords} from '../../models/openroute-coords';


@Component
({
  selector: 'app-tour-dashboard',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './tour-dashboard.html',
  styleUrl: './tour-dashboard.scss',
})

export class TourDashboard
{
  //searchControl = new FormControl('');
  fromSuggestions: any[] = [];
  toSuggestions: any[] = [];

  fromSearchSubject = new Subject<string>();
  toSearchSubject = new Subject<string>();

  selectedFrom?: Coords;
  selectedTo?: Coords;
  //so we can refresh data on the front end immediately while waiting for the "pull"
  toursUIonly: TourDtoAngular[] = [];
  newTour: TourDtoAngular = {id: 0, name: '', description: '', from: '', to: ''};
  newOpenRoute = {transportType:'test',
    toFromCoords:
      {fromCoord: {lat:0,lng:0},
        toCoord: {lat:0,lng:0}}};

  //for storing Tour ID which is currently being edited
  //and a empty TourDTOAngular
  editingTourId: number | null = null;
  editedTour: TourDtoAngular = {} as TourDtoAngular;


  constructor(
    private tourService: TourService,
    //we need change detector because Angular doesnt see new data when switching maps for whatever reason
    //change detector forces the visual update
    private changeDetectorRef: ChangeDetectorRef,
    private routeManager: OpenrouteManager,
    private openrouteService: OpenrouteHttpService
  ) {
  }


  ngOnInit()
  {
    this.getTours();

    interval(5000).pipe(switchMap(() => this.tourService.getTours())).subscribe(toursData =>
    {
      this.toursUIonly = toursData || [];
      this.changeDetectorRef.detectChanges();
    });

    this.fromSearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => this.fromSearch(value));

    this.toSearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(value => this.toSearch(value));

  }

  getTours()
  {
    this.tourService.getTours().subscribe(toursData =>
    {
      this.toursUIonly = toursData || [];
      this.changeDetectorRef.detectChanges();
    });
  }

  addTour(form: NgForm)
  {
    if(form.invalid) return;
    //i need to clone it because it always takes the resetted form from the memory

    const tempOpenRoute = {...this.newOpenRoute};
    console.log(tempOpenRoute);
    const tempTour: TourDtoAngular =
      {
        id: -500,
        name: this.newTour.name,
        description: this.newTour.description,
        from: this.newTour.from,
        to: this.newTour.to,
        openRoute: tempOpenRoute,
        routeInformation: this.newTour.routeInformation,
      };

    this.toursUIonly = [...this.toursUIonly, tempTour];

    form.resetForm();

    this.newTour =
    {
      id: 0,
      name: '',
      description: '',
      from: '',
      to: '',
      routeInformation: ''
    };
    this.newOpenRoute =
      {
         transportType:'reset',
         toFromCoords: {
           fromCoord: {
             lat: 0,
             lng: 0
           },
           toCoord: {
             lat: 0,
             lng: 0
           }
         }
      }

    this.tourService.createTour(tempTour).subscribe(() =>
    {
      this.getTours();
    });
    console.log(tempOpenRoute);

  }

  deleteTour(id: number)
  {
    this.toursUIonly = this.toursUIonly.filter(t => t.id !== id); // filter out tour we want to delete
    this.tourService.deleteTour(id).subscribe();
  }

  editTour(tour: TourDtoAngular) {
    this.editingTourId = tour.id;
    this.editedTour = {...tour};
  }

  cancelEdit() {
    this.editingTourId = null;
  }

  saveTour(id: number) {
    this.toursUIonly = this.toursUIonly.map(t => t.id === id ? {...this.editedTour} : t);
    this.editingTourId = null;

    this.tourService.updateTour(id, this.editedTour).subscribe();
  }
  toSearch(toSearchString: string){
    if (!toSearchString) {
      this.fromSuggestions = [];
      return;
    }

    this.openrouteService.getToSearchResult(toSearchString).subscribe(results => {
      this.toSuggestions = results || [];
      this.changeDetectorRef.detectChanges();
    });
  }
  fromSearch(fromSearchString: string) {
    if (!fromSearchString) {
      this.fromSuggestions = [];
      return;
    }

    this.openrouteService.getFromSearchResult(fromSearchString).subscribe(results => {
      this.fromSuggestions = results || [];
      this.changeDetectorRef.detectChanges();
    });
  }
  selectFrom(marker: Coords) {
    console.log(marker.lat);
    this.newOpenRoute.toFromCoords.fromCoord = marker;
    this.newTour.from = marker.name!;
    this.fromSuggestions = [];
  }

  selectTo(marker: Coords) {
    this.newOpenRoute.toFromCoords.toCoord = marker;
    this.newTour.to = marker.name!;
    this.toSuggestions = [];
  }
}
