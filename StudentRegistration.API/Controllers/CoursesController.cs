using StudentRegistration.BusinessLogic.Services;
using System.Web.Http;

namespace StudentRegistration.API.Controllers
{
    [RoutePrefix("api/courses")]
    public class CoursesController : ApiController
    {
        private readonly CourseService courseService;

        public CoursesController()
        {
            courseService = DependencyConfig.CreateCourseService();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllCourses()
        {
            var courses = courseService.GetAllCourses();
            return Ok(courses);
        }
    }
}