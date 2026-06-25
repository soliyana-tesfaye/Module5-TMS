using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController(TmsDbContext context) : ControllerBase
{
    [HttpGet("top")]
    public async Task<IActionResult> TopCourses(
        CancellationToken cancellationToken = default)
    {
        var result = await context.Enrollments
            .GroupBy(e => new
            {
                e.Course.Id,
                e.Course.Title
            })
            .Select(g => new
            {
                CourseId = g.Key.Id,
                CourseTitle = g.Key.Title,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        return Ok(result);
    }
}