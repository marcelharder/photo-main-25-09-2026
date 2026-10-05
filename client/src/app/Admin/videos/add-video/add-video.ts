import { NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, ReactiveFormsModule, Validators } from '@angular/forms';
import { VideoModel } from '../../../_models/VideoModel';
import { VideoService } from '../../../_services/video-service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-video',
  standalone: true,
  imports: [NgIf,ReactiveFormsModule],
  templateUrl: './add-video.html',
  styleUrl: './add-video.css',
})
export class AddVideo implements OnInit {
  addVideoForm: FormGroup;
  videoService = inject(VideoService);
  route = inject(Router); 
  toast = inject(ToastrService);
  
  vm: VideoModel = {
    id: '',
    title: '',
    description: '',
    filePath: '',
    contentType: '',
    category: '',
    duration: '',
    thumbnailPath: ''
  }
  


  constructor(private fb: FormBuilder) {
    this.addVideoForm = this.fb.group({
      title: ['', Validators.required],
      filePath: ['', Validators.required],
      description: [''],
      thumbnailPath: ['', Validators.required],
      duration: ['', Validators.pattern(/^([0-1]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$/)],
      category: [''],
      contentType: ['video/mp4']
  });
  }
  ngOnInit(): void {}

  Cancel(){this.route.navigate(['/videoList'])}


  onSubmit() {
    if (this.addVideoForm.valid) {
      this.videoService.addVideo(this.addVideoForm.value).subscribe({
        next: (response: any) => {
          this.toast.success('Video added successfully');
          this.addVideoForm.reset();
          this.route.navigate(['/videoList']);
        },error: (error: any) => {
          this.toast.error('Something went wrong',error.error);
          console.log(error);
        }
      });
    } else {
      console.log('Form is invalid');
    }
      
  }

}
