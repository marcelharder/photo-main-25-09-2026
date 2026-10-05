import { CommonModule, NgFor, NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AddCategory } from './add-category/add-category';
import { CategoryList } from './category-list/category-list';

@Component({
  selector: 'app-ManageCategoriesComponent',
  templateUrl: './ManageCategoriesComponent.html',
  standalone: true,
  imports: [NgIf, NgFor, CommonModule, CategoryList, AddCategory]   
})
export class ManageCategoriesComponent implements OnInit {
  listOfCategories: any[] = [];
  showAddPage = false;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.listOfCategories = data['lc'] ?? [];
    });
  }

  addCategory() {
    this.showAddPage = true;
  }

  cancelAdd() {
    this.showAddPage = false;
  }
}
