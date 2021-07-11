import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  //we create component intrinsic event that can be referenced in markup as (cancelRegister)
  //e.g. <app-register (cancelRegister)="cancelRegisterMode($event)"></app-register>
  //In this case we want to propagate child component event result to parent component (home.component)
  //Actually we are clicking child component "Cancel" button that generates (emits) boolean data result:
  // - this.cancelRegister.emit(false)
  //Then we can use in parent "home.component" boolean data of the given event through ($event) param:
  // cancelRegisterMode(eventData: boolean) {
  //  this.registerMode = eventData;
  // }
  @Output() cancelRegister = new EventEmitter();

  //model: any = {};

  registerForm!: FormGroup;

  maxDate!: Date;

  validationErrors: string[] = [];

  constructor(private accountService: AccountService,
     private toastr: ToastrService, private fb: FormBuilder,
     private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date(); //today?
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group( {
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],

      gender: ['male'],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    });
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    //Angular 10

    // return (control: AbstractControl) => {
    //   return control?.value === control?.parent?.controls[matchTo].value
    //     ? null : {isMatching: true}
    // }

    // Angular 11: null - matches, isMatching: true - error
    return (control: AbstractControl)  => {
      const c = control?.parent?.controls as any;
      return (c)
        ? (control?.value === c[matchTo]?.value) ? null : { isMatching: true }
        : null;
    };
  }
  register() {
    //console.log(this.registerForm.value);
    //this.accountService.register(this.model).subscribe(response => {
    this.accountService.register(this.registerForm.value).subscribe(response => {
      //console.log(response);
        //this.cancel();
        this.router.navigateByUrl('/members');
    }, error => {
      //console.log(error);
      //this.toastr.error(error.error);
      this.validationErrors = error;
    })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
