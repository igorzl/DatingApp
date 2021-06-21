import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) {}

  getMembers() {
    //return this.http.get<Member[]>(this.baseUrl + 'users');
    if (this.members.length > 0) return of(this.members); // to return data as Observable!
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map((members) => {
        this.members = members;
        return members;
      })
    );
  }

  getMember(username: string) {
    //return this.http.get<Member>(this.baseUrl + 'users/' + username);
    const member = this.members.find((x) => x.username === username);
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {
    //return this.http.put(this.baseUrl + 'users', member);
    return this.http.put(this.baseUrl + 'users', member).pipe(
      // we have to update the local list of members
      map(() => {
        //() - we have nothing in response from API
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }
}
