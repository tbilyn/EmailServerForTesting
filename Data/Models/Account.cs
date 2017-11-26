using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public List<Message> Messages { get; set; }

        public DateTimeOffset Created { get; set; }

    }
}
