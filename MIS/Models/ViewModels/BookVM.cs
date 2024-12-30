using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MIS.Models.ViewModels
{
    public class BookVM
    {
        public Guid BookId { get; set; }

        [Required]
        [DisplayName("Название книги")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Количество страниц")]
        public int Pages { get; set; }

        [Required]
        [DisplayName("Дата публикации")]
        [DataType(DataType.Date)]
        public DateTime Year { get; set; }

        [Required]
        [DisplayName("Цена")]
        [Range(0, long.MaxValue, ErrorMessage = "Цена должна быть положительным числом.")]
        public long Price { get; set; }

        public List<Guid> SelectedAuthorIds { get; set; }
        public List<Guid> SelectedGenreIds { get; set; }
        public List<Guid> SelectedShelfIds { get; set; }
        public Guid SelectedPublisherId { get; set; }
        public List<SelectListItem> AuthorsList { get; set; }
        public List<SelectListItem> GenresList { get; set; }
        public List<SelectListItem> ShelfsList { get; set; }
        public List<SelectListItem> PublisherList { get; set; }

        public BookVM()
        {
            SelectedAuthorIds = new List<Guid>();
            SelectedGenreIds = new List<Guid>();
            SelectedShelfIds = new List<Guid>();
            AuthorsList = new List<SelectListItem>();
            GenresList = new List<SelectListItem>();
            ShelfsList = new List<SelectListItem>();
            PublisherList = new List<SelectListItem>();
        }
    }
}
