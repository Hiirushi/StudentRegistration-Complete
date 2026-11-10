using System;

namespace StudentRegistration.DataAccess.Models
{
    public class StudentReportModel
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Course { get; set; }
    }
}
