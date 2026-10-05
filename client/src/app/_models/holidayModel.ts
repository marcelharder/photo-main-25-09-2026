export interface GalleryImage {
  id: number;
  imageUrl: string;
  caption?: string;
}

export interface GalleryLocation {
  id: number;
  name: string;
  description?: string;
  latitude?: number;
  longitude?: number;
  images: GalleryImage[];
}

export interface Holiday {
  id: number;
  title: string;
  description?: string;
  coverImageUrl?: string;
  startDate?: string;   // ISO string from API
  endDate?: string;     // ISO string from API
  locations: GalleryLocation[];
}
