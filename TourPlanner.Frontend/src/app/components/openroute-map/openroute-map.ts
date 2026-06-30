import { Component, AfterViewInit, effect, inject, signal} from '@angular/core';
import { OpenrouteManager } from '../../services/openroute/openroute-manager';
import { OpenrouteMapmaker } from '../../services/openroute/openroute-mapmaker';
import {Coords} from '../../models/openroute-coords';

@Component({
  selector: 'app-map-page',
  standalone: true,
  templateUrl: './openroute-map.html',
  styleUrls: ['./openroute-map.scss']
})
export class OpenrouteMap implements AfterViewInit {
  //this saves coordinates and stuff in one class with signals
  readonly mapStorage = inject(OpenrouteManager);

  //this draws the lines on the map
  readonly mapDrawer = inject(OpenrouteMapmaker);

  //is the mapRead to be drawn?
  private readonly mapReady = signal(false);

  async ngAfterViewInit(): Promise<void> {
    await this.mapDrawer.initMap('map');
    this.mapReady.set(true);


  }

  constructor() {
    effect(() => {
      if (!this.mapReady()) return;

      const c = this.mapStorage.center();
      const z = this.mapStorage.zoom();
      this.mapDrawer.setCenter(c.lat, c.lng, z);
    });

    effect(() => {
      if (!this.mapReady()) return;

      const route = this.mapStorage.route();
      if (!route) {
        this.mapDrawer.clearRoute();
        return;
      }

      this.mapDrawer.setMarker('from', route.from.lat, route.from.lng);
      this.mapDrawer.setMarker('to', route.to.lat, route.to.lng);
      var coords: Coords[] = [route.to, route.from];
      this.mapDrawer.setRoute(coords);
    });
  }

  onClearRoute(): void {
    this.mapStorage.clearRoute();
  }
}
