using Microsoft.AspNetCore.Mvc;

namespace WebGESCOMPAH.Middleware
{
    public interface IExceptionHandler
    {
        bool CanHandle(Exception exception);
        (ProblemDetails Problem, int StatusCode) Handle(Exception exception, IHostEnvironment env);
    }

}
