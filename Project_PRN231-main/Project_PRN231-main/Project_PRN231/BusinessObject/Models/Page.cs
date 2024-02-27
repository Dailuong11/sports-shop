using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models
{
    public partial class Page
    {
      
        public int Id { get; set; }
        public int? Total { get; set; }
    }
}
