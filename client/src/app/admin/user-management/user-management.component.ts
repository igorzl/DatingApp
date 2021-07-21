import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  modalRef: BsModalRef;

  constructor(
    private adminService: AdminService,
    private modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe((users) => {
      this.users = users;
    });
  }

  openRolesModal(user: User) {
    const modalConfig = {
      class: 'modal-dialog-centered',
      initialState: {
          user: user,
          roles: this.getRolesArray(user)
      }
    };
    this.modalRef = this.modalService.show(RolesModalComponent, modalConfig);
    //sample of passing params other then "initialState"
    //this.modalRef.content.closeBtnName = 'Close';
    this.modalRef.content.updateSelectedRoles.subscribe(values => {
      const rolesToUpdate = {
        roles: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if(rolesToUpdate) {
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => {
          user.roles = [...rolesToUpdate.roles];
          //user.roles = rolesToUpdate.roles;
        });
      }
    })
  }

  private getRolesArray(user: User) {
    const rolesModal = [];
    const userRoles = user.roles;
    const allRoles: any[] = [
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'},
    ]

    allRoles.forEach(roleAny => {
      let isMatch = false;

      // take care of not "in"!
      for(const userRole of user.roles) {
        if(userRole === roleAny.name) {
          isMatch = true;
          roleAny.checked = true;
          rolesModal.push(roleAny);
          break;
        }
      }

      if(!isMatch) {
        roleAny.checked = false;
        rolesModal.push(roleAny);
      }
    })

    //has 3 properties: 'name', 'value', 'checked'
    return rolesModal;
  }
}
