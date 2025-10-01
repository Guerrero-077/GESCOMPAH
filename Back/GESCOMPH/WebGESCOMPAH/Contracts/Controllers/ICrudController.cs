using Entity.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using WebGESCOMPAH.Contracts.Requests;

namespace WebGESCOMPAH.Contracts.Controllers
{
    // Contrato opcional para controladores CRUD genéricos.
    // Nota: los atributos de routing/produces NO deben ir aquí;
    // MVC no aplica atributos definidos en interfaces.
    public interface ICrudController<TGet, TCreate, TUpdate>
        where TGet : BaseDto
    {
        Task<ActionResult<IEnumerable<TGet>>> Get();
        Task<ActionResult<TGet>> GetById(int id);
        Task<ActionResult<TGet>> Post(TCreate dto);
        Task<ActionResult<TGet>> Put(int id, TUpdate dto);
        Task<IActionResult> Delete(int id);
        Task<IActionResult> DeleteLogic(int id);
        Task<IActionResult> ChangeActiveStatus(int id, ChangeActiveStatusRequest body);
        Task<ActionResult<PagedResult<TGet>>> Query(
            int Page = 1,
            int Size = 20,
            string? Search = null,
            string? Sort = null,
            bool Desc = true,
            IDictionary<string, string>? Filters = null);
    }
}
