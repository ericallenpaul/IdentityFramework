using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityFramework.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Position { get; set; }
    }
}
