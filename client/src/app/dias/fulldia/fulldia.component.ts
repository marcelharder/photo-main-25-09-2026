import { Component, inject, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { ImageService } from '../../_services/image.service';
import { CarouselModel } from '../../_models/CarouselModel';
import { NgIf } from '@angular/common';
import { slideModel } from '../../_models/slideModel';
import { FormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { BreakpointObserver, Breakpoints, BreakpointState} from '@angular/cdk/layout';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-fulldia',
  standalone: true,
  imports: [NgIf,FormsModule],
  templateUrl: './fulldia.component.html',
  styleUrl: './fulldia.component.css',
})
export class FulldiaComponent implements OnInit {
  baseUrl = environment.apiUrl;
  breakpointSub: any;
  numberOfSlides = 0;
  @Input() id = '';
  diaDetails = 0;
  isMobile = true
  accountService = inject(AccountService);
  imgService = inject(ImageService);
  router = inject(Router);
  carouselData: CarouselModel = {
    numberOfImages: 0,
    category: 0,
    ShowR: false,
    ShowL: false,
    nextImageIdR: '',
    nextImageIdL: '',
  };
  sm: slideModel = {
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
    Spare3: '',
  };

  ngOnInit(): void {
    // get the route id
    if (this.id) {
      this.imgService.getCarouselData(this.id).subscribe({
        next: (data) => {
          this.carouselData = data;
        },
      });
    }
     this.breakpointSub = this.breakpointObserver
      .observe([Breakpoints.Handset])
      .subscribe((state: BreakpointState) => {
        // `state.matches` is true when ANY of the queried breakpoints match.
        this.isMobile = state.matches;
        console.log('Handset breakpoint matched?', this.isMobile);
      });
  }

  /**
   *
   */
   constructor(private toastr: ToastrService, private breakpointObserver: BreakpointObserver) { }

  showDiaDetails(){if(this.diaDetails == 1){return true} else {return false}}
  SetDiaDetails(){
    this.getFotoFileDetails(this.id);
    this.diaDetails = 1;}
  HideDiaDetails(){this.diaDetails = 0;}

  backToDiaList(){
    
    
    this.router.navigate(['/diaList/' + this.carouselData.category]);
  
  }


  getSlideNumber(): string {
    let numberLinks = +this.carouselData.nextImageIdL;
    let numberRechts = +this.carouselData.nextImageIdR;
    let currentNo = 0;
    if(this.carouselData.nextImageIdL === null){currentNo = 1;}; 
    if(this.carouselData.nextImageIdR === null){currentNo = this.carouselData.numberOfImages}; 
    if(this.carouselData.nextImageIdL != null && this.carouselData.nextImageIdR != null){currentNo = numberRechts - 1;}
    
    return currentNo.toString();
  }
  
  getFotoFileDetails(id: string){
    this.imgService.getSpecificFileFromId(id).subscribe({
      next: (response)=>{this.sm = response},
      error: (error)=>{console.log(error)}
    })
  }

  uploadDetails(){
  this.imgService.uploadSpecificSlideModel(this.sm).subscribe({
    next: (response)=>{
      this.toastr.success("Uploaded ...");
      this.HideDiaDetails();
    }
  })


  }

  addMyself() {
  const userId = this.accountService.currentUser()!.Id;
  var testArray = this.sm.Spare1.split(",").map(Number);
  // Add only if not already present
  if (!testArray.includes(userId)) {testArray.push(userId);}
  this.sm.Spare1 = testArray.join(",");
  this.uploadDetails();
}
  removeMyself() {
  const userId = this.accountService.currentUser()!.Id;
  let testArray = Array.isArray(this.sm.Spare1)
    ? this.sm.Spare1
    : this.sm.Spare1
        ? this.sm.Spare1.split(",").map(Number)
        : [];
  // Remove the userId
  testArray = testArray.filter(id => id !== userId);
  this.sm.Spare1 = testArray.join(",");
  this.uploadDetails();
}


  leftButtonClicked() {
  
    // link to this page with nextImageIdL queryParam
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigate(['/fulldia/' + this.carouselData.nextImageIdL]);
  }
  rightButtonClicked() {
    
    // link to this page with nextImageIdR queryParam
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.router.onSameUrlNavigation = 'reload';
    this.router.navigate(['/fulldia/' + this.carouselData.nextImageIdR]);
  }
  backToArray() {
    this.router.navigate(['/categoryList']);
  }
  showOnlyOneSlide() {
    if (this.carouselData.numberOfImages == 0) {
      return true;
    } else {
      return false;
    }
  }
  showTheLeftButton() {
    if (this.carouselData.ShowL) {
      return true;
    } else {
      return false;
    }
  }
  showTheRightButton() {
    if (this.carouselData.ShowR) {
      return true;
    } else {
      return false;
    }
  }

  downloadImage() {
    window.open(this.getFotoFile(this.id), '_blank');
  }
  getFotoFile(id: string) {
    return this.baseUrl + 'Images/getFullImageFile/' + id;
  }
}
