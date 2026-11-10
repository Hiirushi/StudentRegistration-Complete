using System;

namespace StudentRegistration.DataAccess.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int CourseId { get; set; }
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
