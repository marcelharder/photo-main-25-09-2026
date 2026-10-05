import { Component, inject, input, Input, OnInit, signal } from '@angular/core';
import { VideoModel } from '../../_models/VideoModel';
import { environment } from '../../../environments/environment';
import { VideoService } from '../../_services/video-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-fullvideo',
  standalone: true,
  imports: [],
  templateUrl: './fullvideo.html',
  styleUrls: ['./fullvideo.css'],
})
export class FullvideoComponent implements OnInit{
  
  private videoService = inject(VideoService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  baseUrl = environment.apiUrl;
  video = input.required<VideoModel>();
  videoId='';
  videoUrl = signal<string | null>(null);
  
ngOnInit(): void {
    this.videoId = this.route.snapshot.paramMap.get('id') ?? '';
    if (!this.videoId) return;

    this.videoService.getVideoById(+this.videoId).subscribe({ 
         next: (blob)  => {
            const url = URL.createObjectURL(blob);
            this.videoUrl.set(url);
         },
         error: (error) => {
            console.log('Error loading video:', error);    
         }
    });
  }
 

  backToVideoList() {
    this.router.navigate(['/videoList']);
  }
}
