import { NgIf } from '@angular/common';
import { Component, inject,OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { VideoModel } from '../../../_models/VideoModel';
import { VideoService } from '../../../_services/video-service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-update-video',
  standalone: true,
  imports: [NgIf,ReactiveFormsModule],
  templateUrl: './update-video.html',
  styleUrl: './update-video.css'
})
export class UpdateVideo implements OnInit {
  updateVideoForm: FormGroup; 
  videoService = inject(VideoService);
  router = inject(Router); 
  route = inject(ActivatedRoute);
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
    this.updateVideoForm = this.fb.group({
      id: [''],
      title: ['', Validators.required],
      filePath: ['', Validators.required],
      description: [''],
      thumbnailPath: ['', Validators.required],
      duration: ['', Validators.pattern(/^([0-1]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$/)],
      category: [''],
      contentType: ['video/mp4']
  });
  }
  ngOnInit(): void {
    // get the query param id and use it to get the video details and patch the form
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.videoService.videoDetails(+id).subscribe({
          next: (response: any) => {
            this.updateVideoForm.patchValue(response);
          },
          error: (error: any) => {
            console.log(error);
          }
        });
      }
    });
  }

  Cancel(){
    this.router.navigate(['/manageVideos']);
  }
onSubmit() {


    if (this.updateVideoForm.valid) {
      this.videoService.updateVideo(this.updateVideoForm.value).subscribe({
        next: (response: any) => {
          this.toast.success('Video updated successfully');
          this.updateVideoForm.reset();
          this.router.navigate(['/manageVideos']);
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
