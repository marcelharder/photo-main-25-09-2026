import { Component, inject, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ActivatedRoute, Router } from '@angular/router';
import { VideoService } from '../../_services/video-service';
import { AccountService } from '../../_services/account.service';
import { FormsModule } from '@angular/forms';
import { VideoDetailsComponent } from '../video-details/video-details';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-video-list',
  standalone: true,
  imports: [PaginationModule, FormsModule, VideoDetailsComponent],
  templateUrl: './video-list.html',
  styleUrl: './video-list.css'
})
export class VideoListComponent implements OnInit {
  baseUrl = environment.apiUrl;
  private route = inject(ActivatedRoute);
  videoService = inject(VideoService);
  accountService = inject(AccountService);
  router = inject(Router);
  categoryId = '1';
  currentPage = 0;
  smallnumPages = 0;
  pageNumber = 1;
  pageSize = 12;
  
  ngOnInit(): void {
    //this.categoryId = this.route.snapshot.params['id']; // get category id from the route wil work once i define categories
    this.pageNumber = this.accountService.currentVideoListNo(); // get the page number from the signal
    this.loadVideos(+this.categoryId);
  }
 loadVideos(id: number) {
    this.videoService.getVideosForCurrentUser(1, this.pageNumber, this.pageSize).subscribe({
      next: () => {}
    });
  }

  pageChanged(event: any) {
    if (this.pageNumber != event.page) {
      this.accountService.currentVideoListNo.set(event.page); // update the signal in the account service
      this.pageNumber = event.page;
      this.loadVideos(+this.categoryId);
    } else {}
  }

  
  

  backToArray() {
    this.router.navigate(['/']);
  }

}
