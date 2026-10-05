import { Component, inject, input } from '@angular/core';
import { slideModel } from '../../_models/slideModel';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dias-details',
  standalone: true,
  imports: [],
  templateUrl: './dias-details.component.html',
  styleUrl: './dias-details.component.css',
})
export class DiasDetailsComponent {
  private router = inject(Router);
  baseUrl = environment.apiUrl;
  dia = input.required<slideModel>();

  getImageFromServer(id: number) {
    return this.baseUrl + 'Images/getImageFile/' + id;
  }
  
  getImageFull(id: number) {
    // go to the full image page
    this.router.navigate(['/fulldia/' + id]);
  }
}
