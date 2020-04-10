using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tut6.Models;

namespace tut6.Services
{
    public interface IStudentServiceDb
    {
        IActionResult Enrollment(Enrollment enrollment);
        IActionResult Promote(int semester, string studies);
    }
}
