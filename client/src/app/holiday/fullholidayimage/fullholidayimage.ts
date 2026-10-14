import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  inject
} from '@angular/core';

import { HolidayService } from '../../_services/holiday-service';
import { NgIf } from '@angular/common';
import { GalleryImage } from '../../_models/holidayModel';
import { AccountService } from '../../_services/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-fullholidayimage',
  standalone: true,
  imports: [NgIf],
  templateUrl: './fullholidayimage.html',
  styleUrl: './fullholidayimage.css'
})
export class Fullholidayimage implements OnInit {
  listOfImages: GalleryImage[] = [];
  router = inject(Router);

  private holidayService = inject(HolidayService);
  private accountService = inject(AccountService);
  id: number = 0;

  ngOnInit(): void {
    //get id fromurl
    const url = window.location.href;
    const idString = url.split('/').pop();
    if (idString) {
      this.id = parseInt(idString, 10);
    }
    // get the list of images from the service
    this.listOfImages = this.accountService.HolidayImageArray() || [];
  }

  get imageUrl(): string {
    return this.holidayService.getImageFromServer(this.id);
  }

  leftButtonClicked(): void {
    // navigate to the previous image
    if (this.id > 1) {
      this.id--;
      window.history.pushState({}, '', `/holidayfullimage/${this.id}`);
    }
  }

  rightButtonClicked(): void {
    // navigate to the next image
    this.id++;
    window.history.pushState({}, '', `/holidayfullimage/${this.id}`);
  }

  showLeftButton(): boolean {
    var firstImageId = this.listOfImages.length > 0 ? this.listOfImages[0].id : 1;
    return this.id > firstImageId;
  }

  showRightButton(): boolean {
    var lastImageId = this.listOfImages.length > 0 ? this.listOfImages[this.listOfImages.length - 1].id : 1;
    return this.id < lastImageId;
  }  
  
  backToHolidayList(): void {
    this.accountService.HolidayImageArray.set(null); // clear the image array in the service
    // navigate back to the holiday list page
   this.router.navigate(['/holidayList']);
  }

}