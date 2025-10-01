using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Middleware
{
    public interface IExceptionHandler
    {
        int Priority { get; }
        bool CanHandle(Exception exception);
        (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env, HttpContext http);
    }

}
