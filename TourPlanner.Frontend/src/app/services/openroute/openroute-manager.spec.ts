import { TestBed } from '@angular/core/testing';

import { OpenrouteManager } from './openroute-manager';

describe('OpenrouteManager', () => {
  let service: OpenrouteManager;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OpenrouteManager);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
