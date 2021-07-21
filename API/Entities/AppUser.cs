using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int> //by default is string
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }

        // public int GetAge() { 
        //     return DateOfBirth.CalculateAge();
        //  }

        //all users she likes
        public ICollection<UserLike> LikedUsers { get; set; }
        //all users that like her
        public ICollection<UserLike> LikedByUsers { get; set; }

        //messages fields
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
        //given user roles
        public ICollection<AppUserRole> Roles { get; set; }
    }
}