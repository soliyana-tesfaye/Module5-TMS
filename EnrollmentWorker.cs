/*
public class EnrollmentWorker
{
    private readonly IEnrollmentService _service;

    // ❌ BUG INTENTIONALLY HERE (singleton depends on scoped)
    public EnrollmentWorker(IEnrollmentService service)
    {
        _service = service;
    }
}
*/

/*
using Microsoft.Extensions.DependencyInjection;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        using var scope = _scopeFactory.CreateScope();

        var service =
            scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // Use the scoped service here
        var enrollments = service.GetAllAsync().Result;

        Console.WriteLine(
            $"Processed {enrollments.Count} enrollment(s).");
    }
}

*/

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        using var scope = _scopeFactory.CreateScope();

        var service =
            scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        //var enrollments = service.GetAllAsync().Result;
        var enrollments = service.GetAllAsync().GetAwaiter().GetResult();

        Console.WriteLine(
            $"Processed {enrollments.Count} enrollment(s).");
    }
}