using Microsoft.Data.SqlClient;
using StudentRegistration.DataAccess.Database;
using System.Data;

namespace StudentRegistration.DataAccess.Repositories
{
    public class CourseRespository
    {
        private readonly DbConnectionFactory dbConnectionFactory;

        public CourseRespository(DbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;   
        }

        public DataTable GetAllCourses()
        {
            DataTable table = new DataTable("Courses");

            using (SqlConnection connection = dbConnectionFactory.CreateConnection())
            {
                string query = "SELECT CourseID, CourseName FROM Courses Order By CourseName";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.Fill(table);
                }
            }
            return table;
        }
    }
}
