import { Component, inject, OnInit, signal } from '@angular/core';
import { VideoService } from '../../../_services/video-service';
import { ActivatedRoute, Router } from '@angular/router';
import { VideoModel } from '../../../_models/VideoModel';
import { AddVideo } from '../add-video/add-video';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaginatedResult } from '../../../_models/pagination';

@Component({
  selector: 'app-manage-videos',
  standalone: true,
  imports: [NgFor, AddVideo, NgIf, FormsModule],
  templateUrl: './manage-videos.html',
  styleUrl: './manage-videos.css'
})
export class ManageVideos implements OnInit {
  videoService = inject(VideoService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  result = signal<PaginatedResult<VideoModel[]> | null>(null);
  addVideoForm: any;

  listOfVideos: VideoModel[] = [];
  AddPage = 0;
  category = 1;

  constructor() { }


  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => {
        this.listOfVideos = data['vid'];

      },
    });
  }

  showAddPage() { if (this.AddPage == 1) { return true; } else { return false; } }

  addVideo() { this.AddPage = 1 }

  editVideo(id: string) { this.router.navigateByUrl("/updateVideo/" + id); }

  getVideosByCategory() {
    this.listOfVideos = [];
    var cat = +this.category;
    this.videoService.videoByCategory(cat).subscribe({
      next: (response: any) => {
        this.listOfVideos = response;
        console.log(this.result());
      },
      error: (error: any) => {
        console.log(error);
      },
    });

  }

}
