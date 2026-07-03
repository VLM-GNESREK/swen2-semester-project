export interface Coords {
  name?: string;
  lat: number;
  lng: number;
}
export interface ToFromCoords {
  fromCoord: Coords;
  toCoord: Coords;
}
export interface OpenRoute{

  transportType: string;
  toFromCoords: ToFromCoords;
  duration?: number;
  distance?: number;
  steps?: Coords[];

}
