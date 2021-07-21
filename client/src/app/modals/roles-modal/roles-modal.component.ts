import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {

  //EventEmitter from '@angular/core'
  @Input() updateSelectedRoles = new EventEmitter();
  //this props will be displayed in our modal
  user: User;
  roles: any[] = [];

  //injected service must be public to be visible by template!
  constructor(public modalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  updateRoles() {
    this.updateSelectedRoles.emit(this.roles);
    this.modalRef.hide();
  }

}
