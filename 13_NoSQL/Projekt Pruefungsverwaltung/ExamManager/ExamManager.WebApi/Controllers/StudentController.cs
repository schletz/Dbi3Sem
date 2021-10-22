using ExamManager.Application.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamManager.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ExamDatabase _db;

        public StudentController(ExamDatabase db)
        {
            _db = db;
        }
        [HttpGet("/api/students")]
        public IActionResult GetStudents()
        {
            var repo = _db.StudentRepository;

            return Ok(repo.Queryable
                .OrderBy(s => s.SchoolClass)
                .ThenBy(s => s.Lastname)
                .ThenBy(s => s.Firstname)
                .ToList()  // Map ist keine MongoDB Expression, daher ToList()
                .Select(s => Mapper.Map(s)));
        }
    }
}
