using System;
using System.Collections.Generic;

namespace MIS.Models.ViewModels
{
    public class EditShelfWithBooksVM
    {
        public Guid ShelfId { get; set; }
        public string Location { get; set; }
        public List<BookOnShelfVM> BooksOnShelf { get; set; }
    }

    public class BookOnShelfVM
    {
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public int Counter { get; set; } 
    }
}
