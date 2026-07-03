import {Component, ChangeDetectorRef} from '@angular/core';
import {RouterLink} from '@angular/router';
import {TourDtoAngular} from '../../models/tourModel';
import {ActivatedRoute} from '@angular/router';
import {OnInit} from '@angular/core';
import {TourService} from '../../services/tourService';
import {OpenrouteMap} from '../../components/openroute-map/openroute-map';
import {OpenrouteManager} from '../../services/openroute/openroute-manager';

@Component
({
  selector: 'app-tour-details',
  imports: [RouterLink, OpenrouteMap],
  templateUrl: './tour-details.html',
  styleUrl: './tour-details.scss',
})

export class TourDetailsComponent implements OnInit {
  constructor(private route: ActivatedRoute,
              private tourService: TourService,
              private changeDetectorRef: ChangeDetectorRef,
              private routeManager: OpenrouteManager) {
  }

  tour: TourDtoAngular | null = null;

  ngOnInit(): void {

    this.route.paramMap.subscribe(params => {
      console.log('Available Route Parameters:', params.keys);
      const idParam = params.get('id');

      if (idParam === null) {
        console.error('No tour ID provided in route parameters.');
        return;
      }

      const id = +idParam; // '+' converts string to number
      console.log('Fetching data for ID:', id);
      this.tourService.getTourById(id).subscribe(
        {
          next: tour => {
            this.tour = tour;
            console.log(this.tour);
            this.routeManager.loadRoute(tour.openRoute!.toFromCoords.fromCoord!, tour.openRoute!.toFromCoords.toCoord!,tour.openRoute?.steps!);
            this.changeDetectorRef.detectChanges();
          },
          error: err => {
            console.error('Error fetching tour details:', err);
          }
        });
    });
  }
  get distanceKm(): number {
    const distance = this.tour!.openRoute?.distance;
    return distance && distance !== 0 ? distance / 1000 : 0;
  }
  get durationHH(): number {
    const seconds = Math.floor(this.tour!.openRoute?.duration ?? 0);
    return Math.floor(seconds / 3600);
  }

  get durationMM(): number {
    const seconds = Math.floor(this.tour!.openRoute?.duration ?? 0);
    return Math.floor((seconds % 3600) / 60);
  }
}
