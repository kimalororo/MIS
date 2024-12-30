using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MIS.Models.ViewModels
{
    public class ShelfVM
    {
        [Required]
        public Guid ShelfId { get; set; }

        [Required]
        [DisplayName("Название стеллажа")]
        public string Location { get; set; }

        [Required]
        [DisplayName("Количество экземпляров")]
        public int TotalBooks { get; set; }

        public List<SelectListItem> BooksList { get; set; }
    }



   
}
