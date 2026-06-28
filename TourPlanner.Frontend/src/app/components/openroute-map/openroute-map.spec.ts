import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenrouteMap } from './openroute-map';

describe('OpenrouteMap', () => {
  let component: OpenrouteMap;
  let fixture: ComponentFixture<OpenrouteMap>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OpenrouteMap],
    }).compileComponents();

    fixture = TestBed.createComponent(OpenrouteMap);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
