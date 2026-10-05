import { Component, inject, Input } from '@angular/core';
import { categoryModel } from '../../../../_models/categoryModel';
import { Router } from '@angular/router';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [],
  templateUrl: './category-list.html',
  styleUrl: './category-list.css'
})
export class CategoryList {
  router = inject(Router);
  @Input() cat: categoryModel = {
    Id: 0,
    Name: '',
    Description: '',
    MainPhoto: 0,
    YearTaken: 0,
    Number_of_images: ''
  };

  editCategory(id: number){
    debugger;
    this.router.navigate(['/editCategory/' + id])}
  
  deleteCategory(id: number){}

}
