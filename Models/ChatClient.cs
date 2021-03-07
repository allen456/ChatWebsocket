using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class ChatClient
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ChatClientId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ChatClientName { get; set; }
    }
}