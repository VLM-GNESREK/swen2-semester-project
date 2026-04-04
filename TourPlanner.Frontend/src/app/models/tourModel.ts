export interface TourDtoAngular
{
  id: number;
  name: string;
  description: string;
  from: string;
  to: string;
  transportType: string;
  //???? NO WE USE 🦅IMPERIAL SYSTEM🦅(jk)
  distance?: number; // maybe in metres?
  // I think number is fine, I bet there are pipes which convert number to hours and minutes on frontend
  estimatedTime?: number; // Provisional, might change to a more complex type later on
  routeInformation?: string; // I know this is the graphical rep., just a placeholder for now
}
