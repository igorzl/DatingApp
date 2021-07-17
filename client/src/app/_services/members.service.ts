import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { ResponseBodyWithPaginationHeaderResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, initPaginationQueryStringParams } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  //storing different query results (pagination/filtering)
  memberCache = new Map();

  userParams: UserParams;
  user: User;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams() {
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }

  //{{url}}/api/users?pageNumber=3&pageSize=5
  getMembers(userParams: UserParams) {
    //caching with paging test
    //18_99_1_5_lastActive_male, //minAge: 18, maxAge: 99, pageNumber: 1, pageSize: 5, orderBy: lastActive, gender: male
    //18_99_2_5_lastActive_male
    //console.log(Object.values(userParams).join('_'));
    var queryParamsKey = Object.values(userParams).join('_');

    if (this.memberCache.has(queryParamsKey))
      return of(this.memberCache.get(queryParamsKey)); //observable

    let params = initPaginationQueryStringParams(
      userParams.pageNumber,
      userParams.pageSize
    );

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    //return this.http.get<Member[]>(this.baseUrl + 'users');
    //if (this.members.length > 0) return of(this.members); // to return data as Observable!

    //to pass our params as part of HTTP request we need to add "options" params to "get" call
    //and then after observing manually decode the parts of response (body and headers)

    //should be observable too
    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(
      map((members) => {
        //this.memberCache[queryParamsKey] = members; //doesn't work
        this.memberCache.set(queryParamsKey, members);
        return members;
      })
    );
  }

  getMember(username: string) {
    //return this.http.get<Member>(this.baseUrl + 'users/' + username);

    //after new caching we have nomore have members array
    // const member = this.members.find((x) => x.username === username);
    // if (member !== undefined) return of(member);

    //console.log(this.memberCache);
    //const member = [...this.memberCache.values()];
    //console.log(member);
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.appliedResult), [])
      .find((member: Member) => member.username === username);

    if (member) {
      return of(member);
    }

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

  //post requires body param, in this case it is empty: {}
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }

  getLikes(predicate: string, pageNumber, pageSize) {
    let params = initPaginationQueryStringParams(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);
  }

  getLikesNaive(predicate: string) {
    return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes?predicate=' + predicate);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

}
