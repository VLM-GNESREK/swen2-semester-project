import { TestBed } from '@angular/core/testing';

import { OpenrouteHttpService } from './openroute-http-service';

describe('OpenrouteHttpService', () => {
  let service: OpenrouteHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OpenrouteHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
