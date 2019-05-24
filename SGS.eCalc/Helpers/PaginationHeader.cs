using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SGS.eCalc.Helpers
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }

        public PaginationHeader(int currentPage, int itemPerPage, int totalItems, int totalPages)
        {
                this.TotalItems = totalItems;
                this.TotalPages = totalPages;
                this.CurrentPage = currentPage;
                this.ItemsPerPage = itemPerPage;
        }
    }
}