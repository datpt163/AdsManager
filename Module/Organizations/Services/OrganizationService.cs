using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Organizations.Requests;
using FBAdsManager.Module.Organizations.Response;
using FBAdsManager.Common.Database.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace FBAdsManager.Module.Organizations.Services
{
    public class OrganizationService : IOrganizationService
    {
        public readonly IUnitOfWork _unitOfWork;

        public OrganizationService(IUnitOfWork unitOfWork)
        {
        _unitOfWork = unitOfWork; 
        }

        public async Task<ResponseService> AddOrganizationAsync(AddOrganizationRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null);
            if (request.Description.Length > 249)
                return new ResponseService("Description must < 250 character", null);

            var organize = _unitOfWork.Organizations.Find(x => (x.Name.Equals(request.Name) && x.DeleteDate == null)).FirstOrDefault();
            if(organize != null)
                return new ResponseService("Tên của hệ thống này đã tồn tại", null);
            var organizeAdd = new Common.Database.Data.Organization() { Name = request.Name, Description = request.Description, UpdateDate = DateTime.Now };
            _unitOfWork.Organizations.Add(organizeAdd);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", organizeAdd);
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize)
        {
            if(pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Skip(skip).Take(pageSize.Value);
                var totalCount = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Count();
                return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.Organizations.Find(x => x.DeleteDate == null).ToListAsync());
        }

        public async Task<ResponseService> GetDetailAsync(Guid id)
        {
            var organization = await _unitOfWork.Organizations.FindOneAsync(c => c.Id == id);
            if(organization == null)
                return new ResponseService("Not found", null);

            return new ResponseService("", organization);
        }

        public async Task<ResponseService> Delete(Guid id)
        {
            var organization = await _unitOfWork.Organizations.Find(c => c.Id == id).Include(c => c.Branches).ThenInclude(c => c.Groups).ThenInclude(c => c.Employees).FirstOrDefaultAsync();
            if (organization == null)
                return new ResponseService("Not found", null);
            organization.DeleteDate = DateTime.Now;

            foreach(var s in organization.Branches)
            {
                foreach(var b in s.Groups)
                {
                    foreach (var c in b.Employees)
                    {
                        c.GroupId = null;
                    }
                    b.BranchId = null;
                }
                s.OrganizationId = null;
            }

            _unitOfWork.Organizations.Update(organization);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> Update(UpdateOrganizationRequest request)
        {
            if(string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);

            if (request.Description.Length > 249)
                return new ResponseService("Description must < 250 character", null);

            var organization = await _unitOfWork.Organizations.FindOneAsync(x => x.Id == request.Id);

            if (organization == null)
                return new ResponseService("This organization not found", null, 404);

            var organize = _unitOfWork.Organizations.Find(x => (x.Name.Equals(request.Name) && x.DeleteDate == null) && !x.Name.Equals(organization.Name)).FirstOrDefault();
            if (organize != null)
                return new ResponseService("Tên của hệ thống này đã tồn tại", null);

            organization.Name = request.Name;
            organization.Description = request.Description;
            organization.UpdateDate = DateTime.Now;
            _unitOfWork.Organizations.Update(organization);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", organization);
        }
    }
}
