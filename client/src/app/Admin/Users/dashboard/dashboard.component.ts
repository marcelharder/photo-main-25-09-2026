import { Component, inject, OnInit } from '@angular/core';
import { User } from '../../../_models/User';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../../_services/user.service';
import { environment } from '../../../../environments/environment';
import { NgFor, NgIf } from '@angular/common';
import { UserdetailsComponent } from '../userdetails/userdetails.component';
import { AddUserComponent } from '../add-user/add-user.component';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [UserdetailsComponent, NgFor,AddUserComponent,NgIf,FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  baseUrl = environment.apiUrl;
  private route = inject(ActivatedRoute);
  private usa = inject(UserService);
  private accountService = inject(AccountService);
  listOfUsers: User[] = [];
  AddPage = 0;
  TitelPageImage = 0;

  constructor(private toast:ToastrService) {}

  ngOnInit(): void {
    // get the titelpage image number
    this.TitelPageImage = this.accountService.titelPageImage();
    this.route.data.subscribe({
      next: (data) => {
        this.listOfUsers = data['us'];
   
      },
    });
  }

  showAddPage(){if(this.AddPage == 1){return true;}else{return false;}}

  addUser(){this.AddPage = 1}

  setTitelPageImage(TitelPageImage: number){
    this.accountService.titelPageImage.set(TitelPageImage);
    this.TitelPageImage = TitelPageImage;
    this.toast.success('Titel page image updated');
  }

}
