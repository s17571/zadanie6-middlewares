using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Exceptions;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        // TODO

        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>()
            {
                new Student{IdStudent=1, FirstName="Jan", LastName="Kowalski", Index="s123"},
                new Student{IdStudent=2, FirstName="Andrzej", LastName="Malewicz", Index="s124"},
                new Student{IdStudent=3, FirstName="Krzysztof", LastName="Andrzejewicz", Index="s125"}
            };
            return Ok(list);
        }
    }
}