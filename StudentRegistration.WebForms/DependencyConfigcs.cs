using StudentRegistration.DataAccess.Database;
using StudentRegistration.DataAccess.Repositories;
using StudentRegistration.BusinessLogic.Services;

namespace StudentRegistration.WebForms
{
    public static class DependencyConfig
    {
        public static StudentService CreateStudentService()
        {
            DbConnectionFactory factory = new DbConnectionFactory();

            StudentRepository repository = new StudentRepository(factory);

            return new StudentService(repository);
        }

        public static CourseService CreateCourseService()
        {
            DbConnectionFactory factory = new DbConnectionFactory();

            CourseRespository repository = new CourseRespository(factory);

            return new CourseService(repository);
        }
    }
}