using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Branches.Responses;
using FBAdsManager.Module.Groups.Requests;
using Microsoft.EntityFrameworkCore;

namespace FBAdsManager.Module.Groups.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> AddAsync(AddGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);
            if (request.Description.Length > 249)
                return new ResponseService("Description < 250", null, 400);

            var branch = await _unitOfWork.Branchs.Find(x => x.Id == request.BranchId).Include(c => c.Groups).FirstOrDefaultAsync();
            if (branch == null)
                return new ResponseService("Branch not found", null, 404);

            var group = branch.Groups.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
            if (group != null)
                return new ResponseService("Tên của đội nhóm này đã tồn tại ở chi nhánh" + branch.Name + " đã tồn tại", null, 400);

            var groupAdded = new Group() { Name = request.Name.Trim(), Description = request.Description, UpdateDate = DateTime.Now, BranchId = request.BranchId };
            _unitOfWork.Groups.Add(groupAdded);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", groupAdded);
        }

        public async Task<ResponseService> Delete(Guid id)
        {
            var group = await _unitOfWork.Groups.Find(c => c.Id == id).Include(c => c.Employees).FirstOrDefaultAsync();
            if (group == null)
                return new ResponseService("Not found", null);
            group.DeleteDate = DateTime.Now;

            var error = "Phải xóa tất cả các thành viên thuộc đội nhóm này trước khi xóa, đội nhóm này hiện đang có các thành viên sau: ";

            foreach (var b in group.Employees)
            {
                if (b.DeleteDate == null)
                    error += (b.Name + " ,");
            }

            if (error != "Phải xóa tất cả các thành viên thuộc đội nhóm này trước khi xóa, đội nhóm này hiện đang có các thành viên sau: ")
                return new ResponseService(error.Substring(0, error.Length - 2), null);

            _unitOfWork.Groups.Update(group);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, Guid? organizationId, Guid? branchId)
        {
            var groups = await _unitOfWork.Groups.Find(x => x.DeleteDate == null).Include(x => x.Branch).ThenInclude(c => c.Organization).ToListAsync();
            bool flag = false;
            var response = new List<Group>();
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);
                int skip = (pageIndex.Value - 1) * pageSize.Value;

                if (branchId != null)
                {
                    var totalCount = groups.Where(x => x.BranchId != null && x.BranchId == branchId).ToList().Count();
                    var groupQuery = groups.Where(x => x.BranchId != null && x.BranchId == branchId).ToList().Skip(skip).Take(pageSize.Value).ToList();
                    return new ResponseService("", groupQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }

                if (organizationId != null)
                {
                    var totalCount = groups.Where(x => x.Branch != null && x.Branch.OrganizationId != null && x.Branch.OrganizationId == organizationId).ToList().Count();
                    var groupQuery = groups.Where(x => x.Branch != null && x.Branch.OrganizationId != null && x.Branch.OrganizationId == organizationId).ToList().Skip(skip).Take(pageSize.Value).ToList();
                    return new ResponseService("", groupQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }
                var totalCount2 = groups.Count();
                var groupQuery2 = groups.ToList().Skip(skip).Take(pageSize.Value).ToList();
                return new ResponseService("", groupQuery2, new PagingResponse(totalCount2, pageIndex.Value, pageSize.Value));
            }

            if (organizationId != null)
            {
                response = groups.Where(x => x.Branch != null && x.Branch.OrganizationId != null && x.Branch.OrganizationId == organizationId).ToList();
                flag = true;
            }
            if (branchId != null)
            {
                response = groups.Where(x => x.BranchId != null && x.BranchId == branchId).ToList();
                flag = true;
            }

            if (flag == true)
                return new ResponseService("", response);
            return new ResponseService("", groups);
        }

        public async Task<ResponseService> Update(UpdateGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);

            if (request.Description.Length > 249)
                return new ResponseService("Description must < 250 character", null, 400);

            var group = await _unitOfWork.Groups.FindOneAsync(x => x.Id == request.Id);

            if (group == null)
                return new ResponseService("This group not found", null, 404);


            var branch = await _unitOfWork.Branchs.Find(x => x.Id == request.BranchId && x.DeleteDate == null).Include(c => c.Groups).FirstOrDefaultAsync();
            if (branch == null)
                return new ResponseService("Không tìm thấy chi nhánh", null, 404);


            if (group.BranchId != null && group.BranchId == request.BranchId)
            {
                var groupQuery = branch.Groups.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null) && !x.Name.Equals(group.Name)).FirstOrDefault();
                if (groupQuery != null)
                    return new ResponseService("Tên của đội nhóm này đã tồn tại ở chi nhánh " + branch.Name, null, 400);
            }
            else
            {
                var groupQuery = branch.Groups.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
                if (groupQuery != null)
                    return new ResponseService("Tên của đội nhóm này đã tồn tại ở chi nhánh " + branch.Name, null, 400);
            }

            group.Name = request.Name.Trim();
            group.Description = request.Description;
            group.UpdateDate = DateTime.Now;
            group.BranchId = request.BranchId;
            _unitOfWork.Groups.Update(group);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", new { id = group.Id, name = group.Name, description = group.Description, updateDate = group.UpdateDate, deleteDate = group.DeleteDate, branchName = branch.Name });
        }
    }
}
