import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;

  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  //here "model" will be received from HTML template
  //from <User> compiler infer generic type for all cases inside this.http.post call
  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model)
      .pipe(
        map((user) => {
          if(user) {
            this.setCurrentUser(user);
          }
          return user;
        })
      );
  }

  //here "model" will be received from HTML template: see "model.username" and "model.password"
  //from <User> compiler infer generic type for all cases inside this.http.post call
  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model)
      .pipe(
        map((user) => {
          if(user) {
            this.setCurrentUser(user);
          }
          return user;
        })
      );
  }

  setCurrentUser(user: User | undefined) {
    user.roles = [];

    //token payload data has field "role" (see token in jwt.io)
    //   "roles": [
    //     "Member",
    //     "Moderator"
    // ]
    const roles = this.getDecodedToken(user.token).role;
    // array of strings or just a string
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(undefined);
  }

  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1])); //token payload
  }
}
