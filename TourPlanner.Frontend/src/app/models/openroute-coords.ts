//interface would still allow mutable objects
//with this nothing can be changed, only read
export type LatLng = Readonly<{ lat: number; lng: number }>;

export type RouteResult = Readonly<{
  from: LatLng;
  to: LatLng;
  points: readonly LatLng[];
}>;
