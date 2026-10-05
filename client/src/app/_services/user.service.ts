import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  constructor() { }

  getAllUsers(){ return this.http.get<User[]>(this.baseUrl + 'User/getAllUsers')  }

  editUser(us: User){return this.http.put<string>(this.baseUrl + 'User/UpdateUser', us,{ responseType: 'text' as 'json' })}

  registerUser(us: User){return this.http.post<string>(this.baseUrl + 'account/register', us)}

  deleteUser(us: number){return this.http.delete<string>(this.baseUrl + 'User/RemoveUser/' + us,{ responseType: 'text' as 'json' })}

  getSpecificUser(us: number){return this.http.get<User>(this.baseUrl + 'User/SpecificUser/' + us)}

}
