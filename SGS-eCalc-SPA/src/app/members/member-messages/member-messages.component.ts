import { Component, OnInit } from '@angular/core';
import { Input } from '@angular/core';
import { Message } from '../../_models/message';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../_services/auth.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private alertify: AlertifyService,
    private route: ActivatedRoute, private authService: AuthService) { }

  ngOnInit() {
    this.loadMessages();
  }
  loadMessages() {
     // + to cast the type of nameid to number
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
    .pipe(
      // tap : allow us to do some thing before subscribe to our method
      tap( messages => {
        for (let i = 0; i < messages.length; i++) {
          if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
              this.userService.markAsRead(currentUserId, messages[i].id );
          }
        }
      })
    )
    .subscribe( messages => {
      this.messages = messages;
    }, error => {
      this.alertify.error(error);
    });
  }
  sendMessage() {
    this.newMessage.recipientId =  this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((message: Message) => {
      // to add message to the end of list
      this.messages.unshift(message);
      this.newMessage.content = '';
    }, error => {
      this.alertify.error(error);
    });
  }
}
