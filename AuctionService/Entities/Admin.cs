using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Entities
{
    public class Admin
    {
        public int Id {get; set;} 
        public string Email {get; set;} = null!;
        public string PasswordHash {get; set;}
    }
}