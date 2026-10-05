import { Component, inject, OnInit } from '@angular/core';
import { categoryModel } from '../../../../_models/categoryModel';
import { CategoryService } from '../../../../_services/category.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';

@Component({
  selector: 'app-add-category',
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule],
  templateUrl: './add-category.html',
  styleUrl: './add-category.css',
})
export class AddCategory implements OnInit {
  addCategoryForm: FormGroup;
  categoryService = inject(CategoryService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  toastr = inject(ToastrService);
  years: number[] = [];
  cat: categoryModel = {
    Id: 0,
    Name: '',
    Description: '',
    MainPhoto: 0,
    YearTaken: 0,
    Number_of_images: '',
  };

  constructor(private fb: FormBuilder) {
    this.addCategoryForm = this.fb.group({
      Id: [''],
      Name: ['', Validators.required],
      Description: ['', Validators.required],
      YearTaken: ['', Validators.required],
      MainPhoto: ['', Validators.required],
      Number_of_images: [''],
    });
  }

  ngOnInit(): void {
    const currentYear = new Date().getFullYear();
  for (let year = currentYear; year >= 1950; year--) {
    this.years.push(year);
  }
  }
  
  onSubmit() {
    if (this.addCategoryForm.valid) {
      this.categoryService
        .createCategory(this.addCategoryForm.value)
        .subscribe({
          next: (response: any) => {
            this.toastr.success('Category added successfully');
            this.addCategoryForm.reset();
            this.router.navigate(['/manageCategories']);
          },
        });
    } else {
      this.toastr.error('Please fill in all required fields');
    }
  }

  Cancel() {
    this.router.navigateByUrl('/manageCategories');
  }
}
