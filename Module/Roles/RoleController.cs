using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Response.ResponseApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Roles
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetList()
        {
            return ResponseOk(_unitOfWork.Roles.GetQuery().ToList());
        }
    }
}
