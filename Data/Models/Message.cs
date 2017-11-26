using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public string Content { get; set; }

        public DateTimeOffset Received { get; set; }
    }
}
