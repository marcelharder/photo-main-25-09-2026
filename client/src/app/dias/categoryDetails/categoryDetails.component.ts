import { Component, Input,inject } from '@angular/core';
import { categoryModel } from '../../_models/categoryModel';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-categoryDetails',
  standalone: true,
  imports: [],
  templateUrl: './categoryDetails.component.html',
  styleUrls: ['./categoryDetails.component.css']
})
export class CategoryDetailsComponent {
  private router = inject(Router);
  baseUrl = environment.apiUrl;

  @Input() selectedCategory: categoryModel = {
    Id: 0,
    Name: "",
    Description: '',
    MainPhoto: 0,
    YearTaken: 0,
    Number_of_images: ''
  };

  goDetails(id: number) { this.router.navigate(['/diaList/'+id]);  }
  getImageFromServer(id: string) { return this.baseUrl + 'Images/getImageFile/' + id;  }
}
