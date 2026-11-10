using StudentRegistration.BusinessLogic.Services;
using System.Web.Http;
using StudentRegistration.DataAccess.Models;
using System;

namespace StudentRegistration.API.Controllers
{
    [RoutePrefix("api/students")]
    public class StudentsController : ApiController
    {
        private readonly StudentService studentService;

        public StudentsController()
        {
            studentService = DependencyConfig.CreateStudentService();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllStudents()
        {
            try
            {
                var students = studentService.GetAllStudents();
                return Ok(students);
            }
            catch(Exception ex) {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetStudentById(int id)
        {
            try
            {
                var student = studentService.GetStudentByID(id);
                if(student == null)
                {
                    return NotFound();
                }

                return Ok(student);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult RegisterStudent([FromBody] Student student)
        {
            try
            {
                studentService.RegisterStudent(student);

                return Ok(new
                {
                    message = "Student registered successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateStudent(int id,[FromBody] Student student)
        {
            try
            {
                if (student == null)
                {
                    return BadRequest();
                }

                student.StudentId = id;
                studentService.UpdateStudent(student);
                return Ok(new
                {
                    message = "Student updated successfully"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteStudent(int id)
        {
            try
            {
                studentService.DeleteStudent(id);
                return Ok(new
                {
                    message = "Student deleted succesfully"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}