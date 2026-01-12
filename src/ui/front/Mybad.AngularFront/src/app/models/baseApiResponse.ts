// Base Api response model, all api responses should extend this.
export interface BaseApiResponse {
    id: number;
    errors: string[];
}