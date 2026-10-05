import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CategoryService } from '../../_services/category.service';
import { CategoryDetailsComponent } from '../categoryDetails/categoryDetails.component';
import {PaginationModule} from 'ngx-bootstrap/pagination';
import { AccountService } from '../../_services/account.service';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';

@Component({
    selector: 'app-categoryList',
    standalone: true,
    templateUrl: './categoryList.component.html',
    styleUrls: ['./categoryList.component.css'],
    imports: [CategoryDetailsComponent,PaginationModule,FormsModule,CommonModule,NgIf]
})
export class CategoryListComponent implements OnInit{
  catservice = inject(CategoryService);
  accountService = inject(AccountService);
 
  smallnumPages = 0;
  pageNumber = 1;
  pageSize = 9;

  ngOnInit(){
   this.pageNumber = this.accountService.currentPageNumber();// get the pageNumber from a signal
   this.loadCategories();
    }

  loadCategories(){
    this.catservice.getAllowedCategories(this.pageNumber, this.pageSize);
  }

  pageChanged(event: any){
    if(this.pageNumber != event.page){
      this.accountService.currentPageNumber.set(event.page); // update the signal in the account service
      this.pageNumber = event.page;
      this.loadCategories();
    }
  }
  

}
