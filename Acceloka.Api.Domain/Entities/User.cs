using System;
using System.Collections.Generic;
using System.Text;

namespace Acceloka.Api.Domains.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string GoogleId { get; set; }
        public string Email { get; set; }
    }
}
