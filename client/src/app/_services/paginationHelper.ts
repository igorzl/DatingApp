import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { ResponseBodyWithPaginationHeaderResult as ResponseBodyWithPaginationHeaderResult } from "../_models/pagination";

export function getPaginatedResult<T>(url, params, http: HttpClient) {
  //remember last pagination response result
  const paginatedResult: ResponseBodyWithPaginationHeaderResult<T> = new ResponseBodyWithPaginationHeaderResult<T>();
  return http.get<T>(url, { observe: 'response', params }).pipe(
    map((response) => {
      paginatedResult.appliedResult = response.body;
      //Uppercase - will return from HttpHeaders
      if (response.headers.get('Pagination') !== null) {
        paginatedResult.pagination = JSON.parse(
          response.headers.get('Pagination')
        );
      }

      return paginatedResult;
    })
  );
}

export function initPaginationQueryStringParams(pageNumber: number, pageSize: number) {
  let params = new HttpParams();

  //add pagination params to our API request
  params = params.append('pageNumber', pageNumber.toString());
  params = params.append('pageSize', pageSize.toString());

  return params;
}
