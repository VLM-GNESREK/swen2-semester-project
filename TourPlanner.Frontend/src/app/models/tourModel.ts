export interface TourDtoAngular 
{
  id: number;
  name: string;
  description: string;
  from: string;
  to: string;
  transportType: string;
  distance?: number; // maybe in metres?
  estimatedTime?: number; // Provisional, might change to a more complex type later on
  routeInformation?: string; // I know this is the graphical rep., just a placeholder for now
}
