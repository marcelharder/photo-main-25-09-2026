import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { CategoryService } from '../_services/category.service';

export const CategoryResolver: ResolveFn<any | null> = (
  route,
  state
) => {
  const categoryService = inject(CategoryService);
  return categoryService.getAllCategories();
};
