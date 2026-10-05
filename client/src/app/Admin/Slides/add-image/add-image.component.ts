import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { slideModel } from '../../../_models/slideModel';
import { ImageService } from '../../../_services/image.service';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-image',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './add-image.component.html',
  styleUrl: './add-image.component.css',
})
export class AddImageComponent implements OnInit {
  addImageForm: FormGroup;
  route = inject(ActivatedRoute);
  router = inject(Router);
  toastr = inject(ToastrService);
  imageService = inject(ImageService);
  categoryId: number | null = null;
  image: slideModel = {
    Id: 0,
    ImageUrl: '',
    YearTaken: '',
    Location: '',
    Familie: '',
    Category: 0,
    Quality: '',
    Series: '',
    Spare1: '',
    Spare2: '',
    Spare3: ''
  }
  constructor(private fb: FormBuilder) {
   
    this.addImageForm = this.fb.group({
      ImageUrl: ['', Validators.required],
      Location: [''],
      YearTaken: ['', Validators.required],
      Familie: [''],
      Quality: [''],
      Series: [''],
      Spare1: [''],
      Spare2: [''],
      Spare3: ['']  
    });
  }


  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
     //create a new image with the category id
    if (idParam) {
      this.categoryId = +idParam;
      this.image.Category = this.categoryId;
    }
  }

  onSubmit() {
    //save the image to the database
    if (this.addImageForm.valid) {
      this.image.Id = 0;
      this.image.ImageUrl = this.addImageForm.value.ImageUrl;
      this.image.Location = this.addImageForm.value.Location;
      this.image.Category = this.categoryId || 0;
      this.image.YearTaken = this.addImageForm.value.YearTaken;
      this.image.Familie = this.addImageForm.value.Familie;
      this.image.Quality = this.addImageForm.value.Quality;
      this.image.Series = this.addImageForm.value.Series;
      this.image.Spare1 = this.addImageForm.value.Spare1;
      this.image.Spare2 = this.addImageForm.value.Spare2;
      this.image.Spare3 = this.addImageForm.value.Spare3;

      this.imageService.createImage(this.image).subscribe({
        next: (response: string) => {
          if(response == "Image added"){
             this.toastr.success('Image added successfully');
             this.router.navigateByUrl('/editCategory/' + this.categoryId);
          }
         
        },
        error: () => {
           this.toastr.error('Failed to add image');
        }
      });
    }
  }

  Cancel(){
    this.toastr.info('Image creation cancelled');
    this.router.navigateByUrl('/editCategory/' + this.categoryId);}
}