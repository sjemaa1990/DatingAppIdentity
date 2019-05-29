import { Component, OnInit, EventEmitter } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { User } from '../../_models/user';
import { Output } from '@angular/core';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  @Output() updateSelectedRoles = new EventEmitter();
  user: User;
  title: string;
  closeBtnName: string;
  roles: any[] = [];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit() {
  }
  updateRoles () {
    this.updateSelectedRoles.emit(this.roles);
    // close the modal
    this.bsModalRef.hide();
  }

}
