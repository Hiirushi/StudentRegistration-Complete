using System.Data;
using StudentRegistration.DataAccess.Repositories;

namespace StudentRegistration.BusinessLogic.Services
{
    public class CourseService
    {
        private readonly CourseRespository courseRespository;

        public CourseService(CourseRespository courseRespository)
        {
            this.courseRespository = courseRespository;
        }

        public DataTable GetAllCourses()
        {
            return courseRespository.GetAllCourses();
        }
    }
}
