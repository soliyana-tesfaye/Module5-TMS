using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.DTOs;
using TmsApi.Entities;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly TmsDbContext _context;

    public StudentsController(TmsDbContext context)
    {
        _context = context;
    }

    // =========================================
    // 1. GET ALL STUDENTS
    // =========================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        var students = await _context.Students
            .AsNoTracking()
            .ToListAsync();

        return Ok(students);
    }

    // =========================================
    // 2. PAGINATION
    // =========================================
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<StudentDto>>> GetPagedStudents(
        int page = 1,
        int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        var query = _context.Students
            .AsNoTracking()
            .OrderBy(s => s.Name);

        var totalCount = await query.CountAsync();

        var students = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                RegistrationNumber = s.RegistrationNumber,
                Name = s.Name,
                GPA = s.GPA,
                IsActive = s.IsActive
            })
            .ToListAsync();

        return Ok(new PagedResult<StudentDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = students
        });
    }

    // =========================================
    // 3. TOP COURSES
    // =========================================
    [HttpGet("top-courses")]
    public async Task<ActionResult> GetTopCourses()
    {
        var result = await _context.Enrollments
            .Include(e => e.Course)
            .GroupBy(e => e.Course.Title)
            .Select(g => new CourseSummaryDto
            {
                Title = g.Key,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync();

        return Ok(result);
    }

    // =========================================
    // 4. CREATE STUDENT (POST)
    // =========================================
    [HttpPost]
    public async Task<ActionResult> CreateStudent([FromBody] StudentDto dto)
    {
        if (dto.RegistrationNumber.Length > 20)
            return BadRequest("RegistrationNumber must be max 20 characters");

        if (dto.Name.Length > 100)
            return BadRequest("Name must be max 100 characters");

        var student = new Student
        {
            RegistrationNumber = dto.RegistrationNumber,
            Name = dto.Name,
            GPA = dto.GPA,
            IsActive = dto.IsActive
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return Ok(student);
    }
}