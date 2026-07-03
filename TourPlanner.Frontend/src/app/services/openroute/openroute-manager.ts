import {Injectable, signal, computed} from '@angular/core';
import {Coords, ToFromCoords} from '../../models/openroute-coords';

@Injectable({
  providedIn: 'root',
})
export class OpenrouteManager {// Vienna defaults
  readonly center = signal<Coords>({lat: 48.2083, lng: 16.3731});
  readonly zoom = signal<number>(12);

  // Demo endpoints
  readonly from = signal<Coords>({lat: 48.23963, lng: 16.37667}); // FH Technikum Wien (approx)
  readonly to = signal<Coords>({lat: 48.20849, lng: 16.37306});   // Stephansdom (approx)

  // Load state
  readonly isLoading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  // Route result (null until loaded)
  route = signal<Coords[] | null>(null);

  // Derived state
  readonly hasRoute = computed(() => this.route() !== null);

  constructor() {
  }

  clearRoute(): void {
    this.route.set(null);
    this.error.set(null);
  }

  loadRoute(from: Coords, to: Coords, points: Coords[]): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.center.set({
      lat: (from.lat! + to.lat!) / 2,
      lng: (from.lng! + to.lng!) / 2,
    });
    this.route.set(points);
    this.from.set(from);
    this.to.set(to);
    this.zoom.set(13);
    this.isLoading.set(false);
  }


}
