import { TestBed } from '@angular/core/testing';

import { OpenrouteMapmaker } from './openroute-mapmaker';

describe('OpenrouteMapmaker', () => {
  let service: OpenrouteMapmaker;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OpenrouteMapmaker);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
