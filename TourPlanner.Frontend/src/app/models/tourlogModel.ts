export interface TourLog
{
    id: number;
    tourID: number;
    date: string; // Or Date if you prefer?
    username: string;
    difficulty: number;
    rating: number;
    totalDistance: number;
    totalTime: number;
}