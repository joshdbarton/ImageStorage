using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStorage.Models.ViewModels
{
    public class PersonFormViewModel
    {
        public Person Person { get; set; }

        [DisplayName("Profile Image")]
        public IFormFile ImageUpload { get; set; }
    }
}
