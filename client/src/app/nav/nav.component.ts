import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BreakpointObserver, Breakpoints, BreakpointState} from '@angular/cdk/layout';
import { NgIf } from '@angular/common';
import { VideoService } from '../_services/video-service';


@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive,NgIf],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit {
  isMobile = false;
  breakpointSub: any;
  accountService = inject(AccountService);
  videoService = inject(VideoService);
  router = inject(Router);
  model: any = {};
  currentUserName? = "";
  ngOnInit(): void { 
    this.breakpointSub = this.breakpointObserver
      .observe([Breakpoints.Handset])
      .subscribe((state: BreakpointState) => {
        // `state.matches` is true when ANY of the queried breakpoints match.
        this.isMobile = state.matches;
        console.log('Handset breakpoint matched?', this.isMobile);
      });
 }

  ngDestroy(): void {
    this.breakpointSub.unsubscribe();
  }

  constructor(private toastr: ToastrService, private breakpointObserver: BreakpointObserver) { }

  login() {
    this.accountService.login(this.model).subscribe({
      next: response => {
        this.currentUserName = this.accountService.currentUser()?.UserName;
       /*  // make sure to register this user in the video side
        this.videoService.registerUserWithVideos().subscribe({
          next: response => {
            console.log(response);
          },
          error: error => { console.log(error);},
          complete: () => { }
        }) */
        
        //route to category list after login
        this.router.navigate(['/categoryList']);
      },
      error: error => { console.log(error);this.toastr.warning('Wrong password') },
      complete: () => { }
    })

    console.log(this.model);
  }

  logout() {
    this.router.navigate(['/']);
    this.accountService.logout();
  }

}
