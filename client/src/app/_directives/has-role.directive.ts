import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

//Angular directives in markup: *ngIf, *ngFor, bsRadio, dropdownToggle
// * - structural directives: *appHasRole='["Admin", "Moderator"]'
@Directive({
  selector: '[appHasRole]',
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];

  user: User;

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
    });
  }

  ngOnInit(): void {
    //if no roles: clear the view
    if(!this.user?.roles || this.user == null) {
      this.viewContainerRef.clear();
      return;
    }

    if(this.user.roles.some(role => this.appHasRole.includes(role))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }
}
