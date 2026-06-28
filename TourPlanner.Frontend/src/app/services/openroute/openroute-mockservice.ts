import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { LatLng,RouteResult } from '../../models/openroute-coords';
import { DEMO_ROUTE_POINTS } from '../../models/demo-route.ors';


@Injectable({
  providedIn: 'root',
})
export class OpenrouteMockservice {  /**
 * Simulated external routing API:
 */
getRoute(from: LatLng, to: LatLng): Observable<RouteResult> {
  const result: RouteResult = {
    from,
    to,
    points: DEMO_ROUTE_POINTS,
  };

  // Keep the "remote call" feel
  return of(result).pipe(delay(400));
}
}
