using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tut6.Models;
using tut6.Services;

namespace tut6.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentServiceDb _service;

        public EnrollmentsController(IStudentServiceDb service)
        {
            _service = service;
        }

        [HttpPost("/promotions")]
        public IActionResult Promotion(Promotion promotion)
        {

            return _service.Promote(promotion.Semester, promotion.Studies);
        }
        [HttpPost]
        public IActionResult Enrollment(Enrollment enrollment)
        {
            return _service.Enrollment(enrollment);
        }
    }
}
