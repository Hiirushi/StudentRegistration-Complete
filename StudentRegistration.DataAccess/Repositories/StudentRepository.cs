using Microsoft.Data.SqlClient;
using StudentRegistration.DataAccess.Database;
using StudentRegistration.DataAccess.Models;
using System;
using System.Data;

namespace StudentRegistration.DataAccess.Repositories
{
    public class StudentRepository
    {
        private readonly DbConnectionFactory dbConnectionFactory;

        public StudentRepository(DbConnectionFactory dbConnectionFactory)
        {
           this.dbConnectionFactory = dbConnectionFactory;
        }

        public bool AddStudent(Student student)
        {
            using (SqlConnection connection = dbConnectionFactory.CreateConnection())
            {
                string query = @"INSERT INTO Students(FirstName, LastName, Address, DateOfBirth, CourseID, IsDeleted, CreatedDate)
                VALUES(@FirstName, @LastName, @Address, @DateOfBirth, @CourseID, 0, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@FirstName", SqlDbType.VarChar, 50).Value = student.FirstName;
                    command.Parameters.Add("@LastName", SqlDbType.VarChar, 50).Value = student.LastName;
                    command.Parameters.Add("@Address", SqlDbType.VarChar, 255).Value =
                        string.IsNullOrWhiteSpace(student.Address)
                            ? (object)DBNull.Value
                            : student.Address;
                    command.Parameters.Add(
                        "@DateOfBirth",
                        SqlDbType.Date
                    ).Value = student.DateOfBirth.HasValue
                        ? (object)student.DateOfBirth.Value.Date
                        : DBNull.Value;
                    command.Parameters.Add("@CourseID", SqlDbType.Int).Value = student.CourseId;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public DataTable GetAllStudents()
        {
            DataTable table = new DataTable("Students");

            using (SqlConnection connection = dbConnectionFactory.CreateConnection())
            {
                string query = @"SELECT s.StudentID, CONCAT(s.FirstName,' ', s.LastName)
                AS [Full Name],
                s.Address, s.DateOfBirth AS [Date of Birth],
                c.CourseName AS Course,
                c.CourseID
                FROM Students s INNER JOIN Courses c ON s.CourseID = c.CourseID
                WHERE s.IsDeleted = 0
                ORDER BY s.StudentID DESC";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }

        public Student GetStudentByID(int studentID)
        {
            using (SqlConnection connection = dbConnectionFactory.CreateConnection())
            {
                string query = @"SELECT StudentID, FirstName, LastName, Address, DateOfBirth, CourseID
                                 FROM Students WHERE StudentID = @StudentID AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student
                            {
                                StudentId = Convert.ToInt32(reader["StudentID"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Address = reader["address"] == DBNull.Value ? "" : reader["Address"].ToString(),
                                DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? (DateTime?)null :
                                                Convert.ToDateTime(reader["DateOfBirth"]),
                                CourseId = Convert.ToInt32(reader["CourseID"])
                            };
                        }
                    }
                }
                return null;
            }
        }

        public bool UpdateStudent(Student student)
        {
            using (SqlConnection connection = dbConnectionFactory.CreateConnection())
            {
                string query = @"UPDATE Students SET FirstName = @FirstName,
                                   LastName = @LastName, Address = @Address, DateOfBirth = @DateOfBirth, CourseID = @CourseID, UpdatedDate = GETDATE()
                                    WHERE StudentID = @StudentID AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(student.Address) ? (object)DBNull.Value
                        : student.Address);
                    command.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth.HasValue ? (object)student.DateOfBirth.Value
                        : DBNull.Value);
                    command.Parameters.AddWithValue("@CourseID", student.CourseId);
                    command.Parameters.AddWithValue("@StudentID", student.StudentId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteStudent(int studentID)
        {
            using (SqlConnection connection = dbConnectionFactory.CreateConnection())
            {
                string query = @"UPDATE Students 
                                SET IsDeleted = 1,
                                    DeletedDate = GETDATE() 
                                WHERE StudentID = @StudentID AND IsDeleted = 0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", studentID);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public DataTable GetStudentsForReport()
        {
            DataTable table = new DataTable();

            using (SqlConnection connection = dbConnectionFactory.CreateConnection())
            {
                string query = @"SELECT s.StudentID,
                s.FirstName,
                s.LastName,
                CONCAT(s.FirstName, ' ', s.LastName) AS FullName,
                s.Address,
                s.DateOfBirth,
                c.CourseName AS Course
                FROM Students s
                INNER JOIN Courses c
                ON s.CourseID = c.CourseID
                WHERE s.IsDelete = 0
                ORDER BY c.CourseName, s.FirstName";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }
    }
}
