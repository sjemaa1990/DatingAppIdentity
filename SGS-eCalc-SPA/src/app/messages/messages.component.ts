import { UserService } from './../_services/user.service';
import { Pagination, PaginatedResult } from './../_models/Pagination';
import { Component, OnInit } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

import { error } from 'util';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';
  constructor(public authService: AuthService, private userService: UserService, private alertifyService: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }
  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid, this.pagination.currentPage,
                   this.pagination.itemsPerPage, this.messageContainer)
                   .subscribe((res: PaginatedResult<Message[]>) => {
                     this.messages = res.result;
                     this.pagination = res.pagination;
                   // tslint:disable-next-line:no-shadowed-variable
                   }, error => {
                     this.alertifyService.error(error);
                   });
  }
  deleteMessage(id: number) {
    this.alertifyService.confirm('Are you sure you want to delete this message', () => {
      // () =>  anonymos function because there are no back result
      this.userService.deleteMessage(id, this.authService.decodedToken.nameid).subscribe(() => {
        this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
        this.alertifyService.success('Message has been deleted successfully');
        // tslint:disable-next-line:no-shadowed-variable
        }, error => {
          this.alertifyService.error('Error appears when deleting message');
        });
    });
  }
  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

}
