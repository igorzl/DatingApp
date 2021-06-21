import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  member: Member = {} as Member;
  user: User = {} as User;
  @ViewChild('editForm') editForm: NgForm = {} as NgForm;

  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private accountService: AccountService,
     private memberService: MembersService, private toastrService: ToastrService) {
    //use the same tactics like in interceptor
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  loadMember() {
    this.memberService.getMember(this.user.username).subscribe(member => this.member = member);
  }

  ngOnInit(): void {
    this.loadMember();
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toastrService.success('Profile updated successfully');
      this.editForm.reset(this.member);
      });
  }

}
