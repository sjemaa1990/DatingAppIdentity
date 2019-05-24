import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  constructor(private authService: AuthService, private userService: UserService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
  }

  sendLike(recipientId: number, user: User) {
    this.userService.sendLike(this.authService.decodedToken.nameid, recipientId).subscribe(data => {
        this.alertify.success('You have liked: ' + user.knownAs);
    }, error => {
      this.alertify.error(error);
    });
  }

}
