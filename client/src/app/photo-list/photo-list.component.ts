import { Component, inject, OnInit } from '@angular/core';
import { ImageService } from '../_services/image.service';
import { AccountService } from '../_services/account.service';
import { DiasDetailsComponent } from '../dias/dias-details/dias-details.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-photo-list',
  standalone: true,
  imports: [DiasDetailsComponent,PaginationModule],
  templateUrl: './photo-list.component.html',
  styleUrl: './photo-list.component.css'
})
export class PhotoListComponent implements OnInit{
  userId = 0;
  currentPage = 0;
  smallnumPages = 0;
  pageNumber = 1;
  pageSize = 12;
  categoryId = "";
  imageService = inject(ImageService);
  accountService = inject(AccountService);
   
  
  ngOnInit(): void {
    this.userId = this.accountService.currentUser()!.Id;
    this.loadImages(this.userId);
  }

  loadImages(id: number){ this.imageService.getDiasForUser(id, this.pageNumber, this.pageSize);}

  pageChanged(event: any){
    if(this.pageNumber != event.page){
      this.pageNumber = event.page;
      this.loadImages(this.userId);
    }
  }

}
