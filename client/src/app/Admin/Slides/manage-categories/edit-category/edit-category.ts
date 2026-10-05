import { Component, inject, OnInit } from '@angular/core';
import { CategoryService } from '../../../../_services/category.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { categoryModel } from '../../../../_models/categoryModel';
import { NgFor, NgIf } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

@Component({
  selector: 'app-edit-category',
  standalone: true,
  imports: [NgIf, NgFor,ReactiveFormsModule],
  templateUrl: './edit-category.html',
  styleUrl: './edit-category.css',
})
export class EditCategory implements OnInit {
  updateCategoryForm: FormGroup;
  categoryService = inject(CategoryService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  toastr = inject(ToastrService);
   years: number[] = []; 

  category: categoryModel = {
    Id: 0,
    Name: '',
    Description: '',
    MainPhoto: 0,
    YearTaken: 0,
    Number_of_images: ''
  };
  constructor(private fb: FormBuilder) {
   
    this.updateCategoryForm = this.fb.group({
      Id: [''],
      Name: ['', Validators.required],
      Description: [''],
      YearTaken: ['', Validators.required],
      MainPhoto: [''],
      Number_of_images: ['']
    });
  }

  ngOnInit(): void {
  const currentYear = new Date().getFullYear();
  for (let year = currentYear; year >= 1950; year--) {
    this.years.push(year);
  }

  const id = this.route.snapshot.paramMap.get('id');
  if (id) {
    this.categoryService.getCategoryById(+id).subscribe({
      next: (cat) => {
        this.category = cat;
        this.updateCategoryForm.patchValue({
          Id: cat.Id,
          Name: cat.Name,
          Description: cat.Description,
          YearTaken: cat.YearTaken,
          MainPhoto: cat.MainPhoto,
          Number_of_images: cat.Number_of_images

        });
      },
    });
  }
}

  onSubmit() {
    console.log(this.updateCategoryForm.value);
    if (this.updateCategoryForm.valid) {
      this.categoryService
        .updateCategory(this.updateCategoryForm.value)
        .subscribe({
          next: (response: any) => {
            this.toastr.success('Category updated successfully');
            this.updateCategoryForm.reset();
            this.router.navigate(['/manageCategories']);
          },
          error: (error: any) => {
            this.toastr.error('Something went wrong', error.message);
            console.log(error);
          },
        });
    } else {
      console.log('Form is invalid');
    }
  }

  addImage(){
    this.router.navigateByUrl('/addImage/' + this.category.Id);
  }

  Cancel() {
    this.router.navigateByUrl('/manageCategories');
  }
}
