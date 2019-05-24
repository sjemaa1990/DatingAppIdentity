export interface Pagination {

    totalItems: number;
    totalPages: number;
    itemsPerPage: number;
    currentPage: number;
}

export class PaginatedResult<T> {
    result: T;
    pagination: Pagination;
}
