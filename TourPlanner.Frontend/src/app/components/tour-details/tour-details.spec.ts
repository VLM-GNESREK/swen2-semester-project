import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourDetails } from './tour-details';

describe('TourDetails', () => {
  let component: TourDetails;
  let fixture: ComponentFixture<TourDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(TourDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
