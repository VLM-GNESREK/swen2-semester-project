import { TestBed } from '@angular/core/testing';

import { OpenrouteMockservice } from './openroute-mockservice';

describe('OpenrouteMockservice', () => {
  let service: OpenrouteMockservice;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OpenrouteMockservice);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
