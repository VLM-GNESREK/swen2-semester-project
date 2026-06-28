import { Injectable,signal,computed} from '@angular/core';
import { LatLng,RouteResult } from '../../models/openroute-coords';
import {OpenrouteMockservice} from '../../services/openroute/openroute-mockservice';

@Injectable({
  providedIn: 'root',
})
export class OpenrouteManager {// Vienna defaults
  readonly center = signal<LatLng>({ lat: 48.2083, lng: 16.3731 });
  readonly zoom = signal<number>(12);

  // Demo endpoints
  readonly from = signal<LatLng>({ lat: 48.23963, lng: 16.37667 }); // FH Technikum Wien (approx)
  readonly to = signal<LatLng>({ lat: 48.20849, lng: 16.37306 });   // Stephansdom (approx)

  // Load state
  readonly isLoading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  // Route result (null until loaded)
  readonly route = signal<RouteResult | null>(null);

  // Derived state
  readonly hasRoute = computed(() => this.route() !== null);

  constructor(private readonly routing: OpenrouteMockservice) {}

  clearRoute(): void {
    this.route.set(null);
    this.error.set(null);
  }

  /**
   * Loads the demo route using a simulated external API.
   */
  loadDemoRoute(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const from = this.from();
    const to = this.to();

    this.routing.getRoute(from, to).subscribe({
      next: r => {
        this.route.set(r);
        // Center roughly between start and destination (demo approach)
        this.center.set({ lat: (from.lat + to.lat) / 2, lng: (from.lng + to.lng) / 2 });
        this.zoom.set(13);
        this.isLoading.set(false);
      },
      error: (e: unknown) => {
        this.error.set(e instanceof Error ? e.message : String(e));
        this.isLoading.set(false);
      },
    });
  }
}
