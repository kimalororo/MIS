using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIS.Models.ViewModels
{
    public class EditUserVM
    {
        public System.Guid UserId { get; set; }

        public string Login { get; set; }
        public string Password{ get; set; }
        [Required]
        [DisplayName("Имя")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public Guid SelectedPostId { get; set; }
        public List<SelectListItem> Posts { get; set; }
    }
}