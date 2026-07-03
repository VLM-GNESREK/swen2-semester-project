import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Coords } from '../../models/openroute-coords';

type LeafletModule = typeof import('leaflet');


@Injectable({
  providedIn: 'root',
})
export class OpenrouteMapmaker {
  //map/leaflet is only rendered on a browser
  //we need platformId to check if browser or not in order to skip
  private readonly platformId = inject(PLATFORM_ID);

  private L: LeafletModule | null = null;
  private map: import('leaflet').Map | null = null;

  // Use vector markers
  private markerFrom: import('leaflet').CircleMarker | null = null;
  private markerTo: import('leaflet').CircleMarker | null = null;

  private routeLine: import('leaflet').Polyline | null = null;

  /**
   * SSR-safe init: Leaflet touches `window` on import -> load only in browser.
   */

  ngAfterViewChecked(): void {
    this.map!.invalidateSize(true);
    //this.map.center = this.center;
  }
  async initMap(containerId: string): Promise<void> {
    //skip if not on browser
    if (!isPlatformBrowser(this.platformId)) return;
    if (this.map) return;

    this.L = await import('leaflet');
    const L = this.L;

    this.map = L.map(containerId, {
      zoomControl: true,
      attributionControl: true,
    });
    // Base tiles (OpenStreetMap)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
      maxZoom: 19,
    }).addTo(this.map);

    // Default view: Vienna
    this.map.setView([48.2083, 16.3731], 12);

  }

  setCenter(lat: number, lng: number, zoom = 13): void {
    this.map?.setView([lat, lng], zoom);
    console.log("IM SETTING CENTER")
  }

  setMarker(kind: 'from' | 'to', coord: Coords): void {
    if (!this.map || !this.L) return;
    const L = this.L;
    console.log("IM SETTING MARKERS")
    const m = L.circleMarker([coord.lat, coord.lng], {
      radius: 8,
      weight: 2,
      opacity: 1,
      fillOpacity: 0.9,
    });

    if (kind === 'from') {
      this.markerFrom?.remove();
      this.markerFrom = m.addTo(this.map);
      this.markerFrom.bindTooltip(coord.name!=null ? coord.name! : 'From', { permanent: true });
    } else {
      this.markerTo?.remove();
      this.markerTo = m.addTo(this.map);
      this.markerTo.bindTooltip(coord.name!=null ? coord.name! : 'To', { permanent: true });
    }
  }

  setRoute(points: readonly Coords[]): void {
    console.log("IM SETTING ROUTE")
    if (!this.map || !this.L) return;
    const L = this.L;

    this.routeLine?.remove();

    const latlngs: import('leaflet').LatLngExpression[] = points.map(p => [p.lat!, p.lng!]);
    this.routeLine = L.polyline(latlngs, { weight: 5, opacity: 0.85 }).addTo(this.map);

    const bounds = this.routeLine.getBounds();
    this.map.fitBounds(bounds, { padding: [20, 20] });
  }

  clearRoute(): void {
    this.routeLine?.remove();
    this.routeLine = null;
  }
}
