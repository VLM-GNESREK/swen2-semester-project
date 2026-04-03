import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TourComponent } from './tour-component';

describe('TourComponent', () => {
  let component: TourComponent;
  let fixture: ComponentFixture<TourComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TourComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TourComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
