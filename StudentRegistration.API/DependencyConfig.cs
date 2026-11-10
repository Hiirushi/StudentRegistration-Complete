
using StudentRegistration.BusinessLogic.Services;
using StudentRegistration.DataAccess.Database;
using StudentRegistration.DataAccess.Repositories;

namespace StudentRegistration.API
{
    public class DependencyConfig
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
            CourseRespository respository = new CourseRespository(factory);
            return new CourseService(respository);
        }
    }
}