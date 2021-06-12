import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if(error) {
          switch (error.status) {
            case 400:
              if(error.error.errors) {
                // still not flattened array
                const modalStateErrors = [];
                for(const key in error.error.errors) {
                  if(error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                //modalStateErrors is array of array
                throw modalStateErrors.flat();
              } else {
                this.toastr.error(error.statusText, error.status); //names taken from inspecting browser
              }
              break;
            case 401:
              //intercepts Login error: see
              // 1) API "AccountController.cs" - return Unauthorized("Invalid user name")
              // 2) nav.component.ts -> login() -> this.toastr.error(error.error) (now removed)
              this.toastr.error(error.error, error.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {
                state: {error: error.error}
              };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected happened');
              console.log(error);
              break;
          }
        }
        //if we don't catch
        return throwError(error);
      })
    );
  }
}
