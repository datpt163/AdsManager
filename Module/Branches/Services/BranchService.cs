using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Branches.Requests;
using FBAdsManager.Module.Organizations.Requests;
using FBAdsManager.Module.Branches.Responses;
using Microsoft.EntityFrameworkCore;
using FBAdsManager.Common.Paging;
namespace FBAdsManager.Module.Branches.Services
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BranchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> AddAsync(AddBranchRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);
            if (request.Description.Length > 249)
                return new ResponseService("Description < 250", null, 400);

            var branch = _unitOfWork.Branchs.Find(x => (x.Name.Equals(request.Name) && x.DeleteDate == null)).FirstOrDefault();
            if (branch != null)
                return new ResponseService("Tên của chi nhánh này đã tồn tại", null, 400);

            var organization = await _unitOfWork.Organizations.FindOneAsync(x => x.Id == request.OrganizationId);
            if (organization == null)
                return new ResponseService("Organization not found", null, 404);

            var branchAdded = new Branch() { Name = request.Name, Description = request.Description, UpdateDate = DateTime.Now, OrganizationId = request.OrganizationId };
            _unitOfWork.Branchs.Add(branchAdded);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", new BranchDTO(branchAdded.Id, branchAdded.Name, branchAdded.Description, branchAdded.UpdateDate, branchAdded.DeleteDate) { OrganizationName = organization.Name });
        }

        public async Task<ResponseService> Delete(Guid id)
        {
            var branch = await _unitOfWork.Branchs.FindOneAsync(c => c.Id == id);
            if (branch == null)
                return new ResponseService("Not found", null);
            branch.DeleteDate = DateTime.Now;


            foreach (var b in branch.Groups)
            {
                foreach (var c in b.Employees)
                {
                    c.GroupId = null;
                }
                b.BranchId = null;
            }

            _unitOfWork.Branchs.Update(branch);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> GetDetailAsync(Guid id)
        {
            var branch = await _unitOfWork.Branchs.Find(c => c.Id == id).Include(c => c.Organization).FirstOrDefaultAsync();
            if (branch == null)
                return new ResponseService("Not found", null);

            if (branch.OrganizationId == null)
                return new ResponseService("", new BranchDTO(branch.Id, branch.Name, branch.Description, branch.UpdateDate, branch.DeleteDate) { OrganizationName = "" });
            return new ResponseService("", new BranchDTO(branch.Id, branch.Name, branch.Description, branch.UpdateDate, branch.DeleteDate) { OrganizationName = branch.Organization.Name });
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, Guid? organizationId)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;

                if (organizationId != null)
                {
                    var pagedOrganizationQuery = _unitOfWork.Branchs.Find(x => (x.DeleteDate == null && x.Organization.Id == organizationId)).Skip(skip).Take(pageSize.Value);
                    var totalCount = _unitOfWork.Branchs.Find(x => (x.DeleteDate == null && x.Organization.Id == organizationId)).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }
                else
                {
                    var pagedOrganizationQuery = _unitOfWork.Branchs.Find(x => x.DeleteDate == null).Skip(skip).Take(pageSize.Value);
                    var totalCount = _unitOfWork.Branchs.Find(x => x.DeleteDate == null).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }
                  
            }

            if(organizationId == null)
                return new ResponseService("", await _unitOfWork.Branchs.Find(x => x.DeleteDate == null).ToListAsync());
            return new ResponseService("", await _unitOfWork.Branchs.Find(x => (x.DeleteDate == null && x.Organization.Id == organizationId)).ToListAsync());
        }
    }
}
