import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from 'src/app/_models/message';
import { Response } from 'selenium-webdriver/http';

// const httpOption = {
//   headers: new HttpHeaders({
//     'Authorization': 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.apiUrl;
constructor(private http: HttpClient) { }

  getUsers(page?, itemPerPage?, userParams?, LikesParams?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    // tslint:disable-next-line:prefer-const
    let params = new HttpParams();
    if (page != null && itemPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemPerPage);
    }

    if (userParams != null ) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (LikesParams === 'Likers' ) {
      params = params.append('Likers', 'true');
    }
    if (LikesParams === 'Likees' ) {
      params = params.append('Likees', 'true');
    }


    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params}).pipe(
      map( response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') != null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      }

      )
    );
  }
  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }
  updateUser(id: number, user: User) {
    return this.http.put<User>(this.baseUrl + 'users/' + id, user);
  }
  setMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }
  sendLike(id: number, recipientId: number) {
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {});
  }
  getMessages(id: number, page?, itemPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemPerPage);
    }
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/message', {observe: 'response', params })
    .pipe(
      map( response => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      }

      )
    );
  }
  getMessageThread (id: number, recipientId: number) {
    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/message/thread/' + recipientId);
  }
  sendMessage (id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/message', message);
  }
  deleteMessage (id: number, userId: number) {
    return this.http.post(this.baseUrl + 'users/' + userId + '/message/' + id, {} );
  }
  markAsRead (userId: number, messageId: number ) {
   this.http.post(this.baseUrl + 'users/' + userId + '/message/' + messageId + '/read', {} )
    .subscribe();
  }
}
