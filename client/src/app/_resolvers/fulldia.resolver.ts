import { ResolveFn } from '@angular/router';
import { CarouselModel } from '../_models/CarouselModel';
import { inject } from '@angular/core';
import { ImageService } from '../_services/image.service';

export const fulldiaResolver: ResolveFn<CarouselModel | null> = (route, state) => {
  const imageService = inject(ImageService);

  const result = route.paramMap.get('id');
  if (!result) return null;

  return imageService.getCarouselData(result);
};
