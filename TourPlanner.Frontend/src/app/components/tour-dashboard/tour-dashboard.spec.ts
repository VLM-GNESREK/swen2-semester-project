import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourDashboard } from './tour-dashboard';

describe('TourDashboard', () => {
  let component: TourDashboard;
  let fixture: ComponentFixture<TourDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourDashboard],
    }).compileComponents();

    fixture = TestBed.createComponent(TourDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
