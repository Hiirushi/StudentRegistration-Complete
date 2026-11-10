using StudentRegistration.DataAccess.Repositories;
using StudentRegistration.DataAccess.Models;
using System;
using System.Data;

namespace StudentRegistration.BusinessLogic.Services
{
    public class StudentService
    {
        private readonly StudentRepository studentRepository;

        public StudentService(StudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }
        private void ValidateStudent(Student student)
        {
            if (student == null)
            {
                throw new Exception("Student details cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(student.FirstName))
            {
                throw new Exception("First Name is Required");
            }

            if (string.IsNullOrWhiteSpace(student.LastName))
            {
                throw new Exception("Last Name is Required");
            }

            if (student.CourseId <= 0)
            {
                throw new Exception("Please Select a Course");
            }

            if (student.FirstName.Length > 50)
            {
                throw new Exception("First Name cannot exceed 50 characters");
            }

            if (student.LastName.Length > 50)
            {
                throw new Exception("Last Name cannot exceed 50 characters");
            }

            if (student.Address != null && student.Address.Length > 255)
            {
                throw new Exception("Address cannot exceed 255 characters");
            }

            if (student.DateOfBirth.HasValue &&
                student.DateOfBirth.Value.Date > DateTime.Today)
            {
                throw new Exception("Date of Birth cannot be in the future");
            }
        }

        public void RegisterStudent(Student student)
        {
            ValidateStudent(student);
            studentRepository.AddStudent(student);
        }

        public DataTable GetAllStudents()
        {
            return studentRepository.GetAllStudents();
        }

        public Student GetStudentByID(int studentID)
        {
            return studentRepository.GetStudentByID(studentID);
        }

        public void UpdateStudent(Student student)
        {
            ValidateStudent(student);
            studentRepository.UpdateStudent(student);
        }

        public void DeleteStudent(int studentID)
        {
            studentRepository.DeleteStudent(studentID);
        }

        public DataTable GetStudentsForReport()
        {
            DataTable students = new DataTable();
            students = studentRepository.GetStudentsForReport();

            return students;
        }
    }
}
