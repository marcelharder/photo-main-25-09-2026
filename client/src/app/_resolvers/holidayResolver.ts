import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { HolidayService } from '../_services/holiday-service';
import { HolidayGalleryResponse } from '../_models/PagedHolidayResult';

export const holidayResolver: ResolveFn<HolidayGalleryResponse | null> = (
  route,
  state
) => {
  const holidayService = inject(HolidayService);
  return holidayService.getGallery(1, 10);
};
