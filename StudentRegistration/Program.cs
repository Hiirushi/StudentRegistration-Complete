using System;
using System.Windows.Forms;
using StudentRegistration.BusinessLogic.Services;
using StudentRegistration.DataAccess.Database;
using StudentRegistration.DataAccess.Repositories;

namespace StudentRegistration
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create database connection factory
            DbConnectionFactory dbConnectionFactory =
                new DbConnectionFactory();

            // Create repositories
            StudentRepository studentRepository =
                new StudentRepository(dbConnectionFactory);

            CourseRespository courseRepository =
                new CourseRespository(dbConnectionFactory);

            // Create services
            StudentService studentService =
                new StudentService(studentRepository);

            CourseService courseService =
                new CourseService(courseRepository);

            // Pass both services to the form
            Application.Run(
                new StudentRegistration(
                    studentService,
                    courseService
                )
            );
        }
    }
}