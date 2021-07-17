import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, initPaginationQueryStringParams } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMessages(pageNumber, pageSize, boxType) {
    let params = initPaginationQueryStringParams(pageNumber, pageSize);

    params = params.append('boxType', boxType);

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  //see API MessagesController => GetMessageThread (will not use pagination, to keep simple - homework)
  getMessageThread(username: string) {
    return this.http.get(this.baseUrl + 'messages/thread/' + username);
  }

  sendMessage(recipientUsername: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', {recipientUsername: recipientUsername,
      content: content});
  }
  
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
