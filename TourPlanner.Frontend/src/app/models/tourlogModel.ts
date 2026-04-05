export interface TourLog
{
    id: number;
    tourID: number;
    // string is fine, for whatever reason back end can auto parse it
    date: string; // Or Date if you prefer?
    username: string;
    comment?: string;
    difficulty: number;
    rating: number;
    totalDistance: number;
    totalTime: number;
}
