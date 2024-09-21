using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Response.ResponseService;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace FBAdsManager.Module.BM.Services
{
    public class BmService : IBmService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BmService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> GetListAsync(Guid? groupId)
        {
            var bms = await _unitOfWork.Pms.GetQuery().Include(c => c.User).ThenInclude(c => c.Group).ToListAsync();
            if (groupId != null)
                bms = bms.Where(x => x.User != null && x.User.GroupId != null && x.User.GroupId == groupId).ToList();
            return new ResponseService("", bms);
        }
    }
}
