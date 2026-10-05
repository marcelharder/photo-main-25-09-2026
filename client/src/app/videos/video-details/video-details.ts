import { Component, inject, input, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Router } from '@angular/router';
import { VideoModel } from '../../_models/VideoModel';

@Component({
  selector: 'app-video-details',
  standalone: true,
  imports: [],
  templateUrl: './video-details.html',
  styleUrls: ['./video-details.css'],
})
export class VideoDetailsComponent  {
  
  baseUrl = environment.apiUrl;
  private router = inject(Router);
  video = input.required<VideoModel>();

  

  getVideoFull(id: string) {this.router.navigate(['/fullVideo', this.video().id]);} 
}
