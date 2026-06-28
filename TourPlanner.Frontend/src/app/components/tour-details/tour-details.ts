import {Component, ChangeDetectorRef} from '@angular/core';
import { RouterLink } from '@angular/router';
import { TourDtoAngular } from '../../models/tourModel';
import { ActivatedRoute } from '@angular/router';
import { OnInit } from '@angular/core';
import { TourService } from '../../services/tourService';
import { OpenrouteMap } from '../../components/openroute-map/openroute-map';

@Component
({
  selector: 'app-tour-details',
  imports: [RouterLink,OpenrouteMap],
  templateUrl: './tour-details.html',
  styleUrl: './tour-details.scss',
})

export class TourDetailsComponent implements OnInit
{
  constructor(private route: ActivatedRoute, private tourService: TourService,
              private changeDetectorRef: ChangeDetectorRef) {}

  tour : TourDtoAngular | null = null;

  ngOnInit(): void
  {

    this.route.paramMap.subscribe(params =>
    {
      console.log('Available Route Parameters:', params.keys);
      const idParam = params.get('id');

      if (idParam === null)
      {
        console.error('No tour ID provided in route parameters.');
        return;
      }

      const id = +idParam; // '+' converts string to number
      console.log('Fetching data for ID:', id);
      this.tourService.getTourById(id).subscribe(
      {
        next: tour =>
        {
          this.tour = tour;
          this.changeDetectorRef.detectChanges();
        },
        error: err =>
        {
          console.error('Error fetching tour details:', err);
        }
      });
    });
  }
}
