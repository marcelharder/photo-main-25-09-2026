import { Component, inject, OnInit } from '@angular/core';
import { environment } from '../../environments/environment';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit{
  baseUrl = environment.apiUrl; 
  accountService = inject(AccountService);

  
  ngOnInit(): void {
    
  }

  getFotoFile() {
    if(this.accountService.titelPageImage() !== 0){
      return this.baseUrl + 'Images/getFullImageFile/' + this.accountService.titelPageImage();
    }
    return 'assets/2.jpg';
  }

  


}
