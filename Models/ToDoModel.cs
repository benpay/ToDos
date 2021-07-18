using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToDos.Models
{
    public class ToDoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        [MaxLength(128)]
        public string CategoryName { get; set; }
    }
}
