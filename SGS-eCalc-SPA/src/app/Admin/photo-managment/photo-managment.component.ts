import { Component, OnInit } from '@angular/core';
import { Photo } from '../../_models/Photo';
import { AlertifyService } from '../../_services/alertify.service';
import { AdminService } from '../../_services/admin.service';

@Component({
  selector: 'app-photo-managment',
  templateUrl: './photo-managment.component.html',
  styleUrls: ['./photo-managment.component.css']
})
export class PhotoManagmentComponent implements OnInit {
  photos: Photo[];
  constructor(private alertfy: AlertifyService, private adminService: AdminService) { }

  ngOnInit() {
    this.getUnapprovedPhoto();
  }
  getUnapprovedPhoto () {
    this.adminService.getPhotosForModeration().subscribe((result: Photo[]) => {
      debugger;
      this.photos = result;
    }, error => {
      this.alertfy.error(error);
    });
  }
  setAsApproved (photoId: number) {

  }
}
