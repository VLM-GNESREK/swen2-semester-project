import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourDetailsComponent } from './tour-details';

describe('TourDetailsComponent', () => {
  let component: TourDetailsComponent;
  let fixture: ComponentFixture<TourDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourDetailsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TourDetailsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
