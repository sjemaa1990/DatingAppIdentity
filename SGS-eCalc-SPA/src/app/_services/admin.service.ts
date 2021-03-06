import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUsersWithRoles () {
    return this.http.get(this.baseUrl + 'admin/GetUsersWithRoles');
  }
  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
  }
  getPhotosForModeration () {
    return this.http.get(this.baseUrl + 'admin/getPhotosForModeration');
  }
}
