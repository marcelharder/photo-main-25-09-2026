import { Holiday } from "./holidayModel";

export interface PagedHolidayResult<T> {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: T[];
}

export type HolidayGalleryResponse = PagedHolidayResult<Holiday>;
