import {ChangeDetectorRef, Component} from '@angular/core';
import { TourDtoAngular } from '../../models/tourModel';
import { ActivatedRoute } from '@angular/router';
import { OnInit } from '@angular/core';
import { TourService } from '../../services/tourService';

@Component
({
  selector: 'app-tour-details',
  imports: [],
  templateUrl: './tour-details.html',
  styleUrl: './tour-details.scss',
})

export class TourDetailsComponent implements OnInit
{
  constructor(private route: ActivatedRoute, private tourService: TourService,private changeDetectorRef: ChangeDetectorRef) {}

  tour : TourDtoAngular | null = null;

  ngOnInit(): void
  {

    this.route.paramMap.subscribe(params =>
    {

      const idParam = params.get('id');
      console.log(idParam);
      if (idParam === null)
      {
        console.error('No tour ID provided in route parameters.');
        return;
      }

      const id = +idParam; // '+' converts string to number
      this.tourService.getTourById(id).subscribe(tour =>
      {
        this.tour = tour;
        this.changeDetectorRef.detectChanges();
        
      });
      console.log(this.tour);

    });
  }
}
