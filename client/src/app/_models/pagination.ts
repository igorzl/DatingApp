export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class ResponseBodyWithPaginationHeaderResult<T> {
  appliedResult: T;
  pagination: Pagination;
}
