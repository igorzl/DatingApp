import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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
        this.router.navigateByUrl('/members');
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

  constructor(
    public accountService: AccountService,
    private router: Router) { }

  ngOnInit(): void {
  }

}
