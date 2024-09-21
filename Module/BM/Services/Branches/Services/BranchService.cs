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

            var organization = await _unitOfWork.Organizations.Find(x => x.Id == request.OrganizationId).Include(c => c.Branches).FirstOrDefaultAsync();
            if (organization == null)
                return new ResponseService("Organization not found", null, 404);

            var branch = organization.Branches.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
            if (branch != null)
                return new ResponseService("Tên của chi nhánh này đã tồn tại ở hệ thống " + organization.Name, null, 400);

            var branchAdded = new Branch() { Name = request.Name.Trim(), Description = request.Description, UpdateDate = DateTime.Now, OrganizationId = request.OrganizationId };
            _unitOfWork.Branchs.Add(branchAdded);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", new BranchDTO(branchAdded.Id, branchAdded.Name, branchAdded.Description, branchAdded.UpdateDate, branchAdded.DeleteDate) { OrganizationName = organization.Name });
        }

        public async Task<ResponseService> Delete(Guid id)
        {
            var branch = await _unitOfWork.Branchs.Find(c => c.Id == id).Include(c => c.Groups).ThenInclude(c => c.Employees).FirstOrDefaultAsync();
            if (branch == null)
                return new ResponseService("Not found", null);

            var error = "Phải xóa tất cả các đội nhóm thuộc chi nhánh này trước khi xóa, chi nhánh này hiện đang có các đội nhóm sau: ";
            foreach (var b in branch.Groups)
            {
                if(b.DeleteDate == null)
                    error += (b.Name + ", ");
            }
            if(error != "Phải xóa tất cả các đội nhóm thuộc chi nhánh này trước khi xóa, chi nhánh này hiện đang có các đội nhóm sau: ")
                return new ResponseService(error.Substring(0, error.Length - 2), null);
            branch.DeleteDate = DateTime.Now;
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
                    var pagedOrganizationQuery = _unitOfWork.Branchs.Find(x => (x.DeleteDate == null && x.Organization != null && x.Organization.Id == organizationId)).OrderByDescending(c => c.UpdateDate).Skip(skip).Take(pageSize.Value).Include(x => x.Organization);
                    var totalCount = _unitOfWork.Branchs.Find(x => (x.DeleteDate == null && x.Organization != null && x.Organization.Id == organizationId)).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }
                else
                {
                    var pagedOrganizationQuery = _unitOfWork.Branchs.Find(x => x.DeleteDate == null).OrderByDescending(c => c.UpdateDate).Skip(skip).Take(pageSize.Value).Include(x => x.Organization);
                    var totalCount = _unitOfWork.Branchs.Find(x => x.DeleteDate == null).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }
            }

            if (organizationId == null)
                return new ResponseService("", await _unitOfWork.Branchs.Find(x => x.DeleteDate == null).Include(x => x.Organization).OrderByDescending(c => c.UpdateDate).ToListAsync());
            return new ResponseService("", await _unitOfWork.Branchs.Find(x => (x.DeleteDate == null && x.Organization != null && x.Organization.Id == organizationId)).OrderByDescending(c => c.UpdateDate).Include(x => x.Organization).ToListAsync());
        }

        public async Task<ResponseService> Update(UpdateBranchRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);

            if (request.Description.Length > 249)
                return new ResponseService("Description must < 250 character", null, 400);

            var branch = await _unitOfWork.Branchs.FindOneAsync(x => x.Id == request.Id);

            if (branch == null)
                return new ResponseService("This branch not found", null, 404);

            var organize = await _unitOfWork.Organizations.Find(x => x.Id == request.OrganizationId && x.DeleteDate == null).Include(c => c.Branches).FirstOrDefaultAsync();
            if (organize == null)
                return new ResponseService("Không tìm thấy hệ thống", null, 404);


            if (branch.OrganizationId != null && branch.OrganizationId == request.OrganizationId)
            {
                var branchQuery = organize.Branches.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null) && !x.Name.Equals(branch.Name)).FirstOrDefault();
                if (branchQuery != null)
                    return new ResponseService("Tên của chi nhánh này đã tồn tại ở hệ thống " + organize.Name, null, 400);
            }
            else
            {
                var branchQuery = organize.Branches.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
                if (branchQuery != null)
                    return new ResponseService("Tên của chi nhánh này đã tồn tại ở hệ thống " + organize.Name, null, 400);
            }

            branch.Name = request.Name.Trim();
            branch.Description = request.Description;
            branch.UpdateDate = DateTime.Now;
            branch.OrganizationId = request.OrganizationId;
            _unitOfWork.Branchs.Update(branch);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", new { id = branch.Id, name = branch.Name, description = branch.Description, updateDate = branch.UpdateDate, deleteDate = branch.DeleteDate, organizationName = organize.Name });
        }
    }
}
