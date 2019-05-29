import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';


@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private alertifyService: AlertifyService, private authService: AuthService, private router: Router) {}
  canActivate(next: ActivatedRouteSnapshot): boolean {
    // ActivatedRouteSnapshot to retreive data of route , retreive roles
    const roles = next.firstChild.data['roles'] as Array<string>;
    if (roles) {
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      } else {
        this.router.navigate(['members']);
        this.alertifyService.error('you are not authorised to access this area');
      }
    }
    if (this.authService.loggedIn()) {
      return true;
    }
    this.alertifyService.error('You shall not access');
    this.router.navigate(['/home']);
    return false;
  }
}
