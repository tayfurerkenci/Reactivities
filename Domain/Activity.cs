using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public bool IsCancelled { get; set; }
        
        // when we creating activity we'll get error
        // activity class require attendee object because of that we initialize that object by default 
        public ICollection<ActivityAttendee> Attendees {get;set;} = new List<ActivityAttendee>(); 
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}