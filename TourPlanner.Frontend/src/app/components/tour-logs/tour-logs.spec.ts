import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourLogs } from './tour-logs';

describe('TourLogs', () => {
  let component: TourLogs;
  let fixture: ComponentFixture<TourLogs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourLogs],
    }).compileComponents();

    fixture = TestBed.createComponent(TourLogs);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
