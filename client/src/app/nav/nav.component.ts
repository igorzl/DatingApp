import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  login() {
    console.log('NavComponent, before login(): ' + this.model);
    this.accountService.login(this.model).subscribe(
      (response) => {
        console.log('NavComponent, login() successfull for: ' + response?.username);
    }, error => {
      console.log(error);
    }, (complete: void) => {
      console.log('NavComponent, login() completed');
    });
  }

  logout() {
    this.accountService.logout();
  }

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
  }

}
