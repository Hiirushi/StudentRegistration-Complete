using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web.Http;

namespace StudentRegistration.API.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        [HttpGet]
        [Route("student-details")]
        public async Task<HttpResponseMessage> GetStudentDetailsReport()
        {
            string reportUrl =
                "http://localhost/ReportServer?/StudentRegistration/StudentDetailsReport&rs:Command=Render&rs:Format=PDF";

            try
            {
                using (var handler = new HttpClientHandler())
                {
                    handler.UseDefaultCredentials = true;

                    using (var client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromMinutes(2);

                        HttpResponseMessage ssrsResponse = await client.GetAsync(reportUrl);

                        if (!ssrsResponse.IsSuccessStatusCode)
                        {
                            string error = await ssrsResponse.Content.ReadAsStringAsync();

                            return Request.CreateErrorResponse(
                                HttpStatusCode.BadGateway,
                                "SSRS returned" +
                                (int)ssrsResponse.StatusCode + ":" + error
                            );
                        }
                        byte[] pdfBytes = await ssrsResponse.Content.ReadAsByteArrayAsync();

                        var response = Request.CreateResponse(HttpStatusCode.OK);

                        response.Content = new ByteArrayContent(pdfBytes);

                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
                        {
                            FileName = "StudentDetailsReport.pdf"
                        };
                        return response;
                    }
                }
            }
            catch (Exception ex) {
                return Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError,
                    ex
                );
            }
        }
    }
}