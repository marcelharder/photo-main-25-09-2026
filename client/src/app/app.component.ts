import { HttpClient } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from "./nav/nav.component";
import { AccountService } from './_services/account.service';
import { HomeComponent } from "./home/home.component";

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    imports: [RouterOutlet, NavComponent, HomeComponent]
})
export class AppComponent implements OnInit {
  
  http = inject(HttpClient);
  private accountService = inject(AccountService);
  title = 'client';
  result: any;

  constructor(){
    window.onbeforeunload = function(){
      localStorage.clear();
      return '';

    }
  }


  ngOnInit(): void {
   this.setCurrentUser();
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.accountService.currentUser.set(user);
  }

  getDescription(){
    this.http.get<string>('http://localhost:8103/api/Config/getDescription/4', {responseType: 'text' as 'json'}).subscribe({
      next: response => this.result = response,
      error: error=> console.log(error),
      complete: ()=> console.log('Request has completed')
    })
  }
}
