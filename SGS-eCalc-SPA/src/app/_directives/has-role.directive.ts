import { Directive, Input, ViewContainerRef, TemplateRef, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
// structure directive
@Directive({
  selector: '[appHasRole]'  // to call this we nee to add *  ==> *appHasRole   // * =>convert element to ng template
})
export class HasRoleDirective implements OnInit { // implement on inti to access to this life cycle
  // to pass it as parameter  adding input
  @Input() appHasRole: string[];
  isVisible = false; // to check the current situation of the element if it s visibale or no
  constructor(private viewContainerRef: ViewContainerRef,   // container for component or template
              private templateRef: TemplateRef<any>,
              private authService: AuthService) { }

  ngOnInit () {
    const userRoles = this.authService.decodedToken.role as Array<string>;
    // if no roles clear the view containerRef
    if (!userRoles) {
      this.viewContainerRef.clear();
    }
    // if user has needed role then render the element
    if (this.authService.roleMatch(this.appHasRole)) {
      if (!this.isVisible) {
        this.isVisible = true;
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.isVisible = false;
        this.viewContainerRef.clear();
      }
    }
  }

}
