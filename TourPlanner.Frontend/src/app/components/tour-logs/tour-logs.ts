import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {CommonModule} from '@angular/common';
import {FormsModule, NgForm} from '@angular/forms';
import { TourLogService } from '../../services/tourlogService';
import { TourLog } from '../../models/tourlogModel';

@Component
({
  selector: 'app-tour-logs',
  imports: [CommonModule, FormsModule],
  templateUrl: './tour-logs.html',
  styleUrl: './tour-logs.scss'
})

export class TourLogsComponent implements OnInit
{
  tourId: number = 0;
  logs: TourLog[] = [];
  editingLogId: number | null = null;
  editedLog: TourLog = {} as TourLog;

  newLog: TourLog =
  {
    id: 0,
    tourID: 0,
    date: new Date().toISOString().split('T')[0],
    username: '',
    difficulty: 0,
    totalDistance: 0,
    totalTime: 0,
    rating: 0,
    comment:''
  };

  constructor(private route: ActivatedRoute, private tourLogService: TourLogService,
              private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit(): void
  {
    this.route.paramMap.subscribe(params =>
    {
      const idParam = params.get('id');

      if (idParam === null)
      {
        console.error('No tour ID provided.');
        return;
      }

      this.tourId = +idParam;
      this.newLog.tourID = this.tourId;
      this.loadLogs();
    });
  }

  loadLogs(): void
  {
    this.tourLogService.getTourLogsByTourId(this.tourId).subscribe(data =>
    {
      this.logs = data || [];
      this.changeDetectorRef.detectChanges();
    });
  }

  addLog(form: NgForm): void
  {
    if(form.invalid) return;
    const tempLog: TourLog =
    {
      ...this.newLog,
      id: -500,
    };

    this.logs = [...this.logs, tempLog];


    form.resetForm();

    this.newLog =
    {
      id: 0,
      tourID: this.tourId,
      date: new Date().toISOString().split('T')[0],
      username: '',
      difficulty: 0,
      totalDistance: 0,
      totalTime: 0,
      rating: 0,
      comment:''
    };

    this.tourLogService.createTourLog(this.tourId, tempLog).subscribe(() =>
    {
      this.loadLogs();
    });
  }

  deleteLog(logId: number): void
  {
    this.tourLogService.deleteTourLog(this.tourId, logId).subscribe(() =>
    {
      this.loadLogs();
    });
  }

  editLog(Log: TourLog): void
  {
    this.editingLogId = Log.id;
    this.editedLog = { ...Log };
  }

  cancelEdit(): void
  {
    this.editingLogId = null;
  }

  saveLog(logID: number): void
  {
     this.logs = this.logs.map(l => l.id === logID ? { ...this.editedLog } : l);
     this.editingLogId = null;
     this.tourLogService.updateTourLog(this.tourId, logID, this.editedLog).subscribe();
   }
}
