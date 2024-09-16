using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Employees.Requests;
using FBAdsManager.Module.Groups.Requests;
using Microsoft.EntityFrameworkCore;

namespace FBAdsManager.Module.Employees.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> AddAsync(AddEmployeeRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);
            if (request.Description.Length > 249)
                return new ResponseService("Description < 250", null, 400);
            if(request.Phone.Length < 9)
                return new ResponseService("Phone not correct", null, 400);
            if (!request.Email.Contains("@"))
                return new ResponseService("Email not correct", null, 400);

            var group = await _unitOfWork.Groups.Find(x => x.Id == request.GroupId).Include(c => c.Employees).FirstOrDefaultAsync();
            if (group == null)
                return new ResponseService("group not found", null, 404);

            var employee = group.Employees.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
            if (employee != null)
                return new ResponseService("Tên của thành viên này đã tồn tại ở đội nhóm: " + group.Name, null, 400);

            var employeeCHeckGmail = _unitOfWork.Employees.Find(x => (x.Email != null && x.Email.Trim().ToUpper().Equals(request.Email.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
            if (employeeCHeckGmail != null)
                return new ResponseService("Gmail này đã được một thành viên khác dùng", null, 400);

            var employeeCHeckPhone = _unitOfWork.Employees.Find(x => (x.Phone != null && x.Phone.Trim().ToUpper().Equals(request.Phone.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
            if (employeeCHeckPhone != null)
                return new ResponseService("Số điện thoại này đã được một thành viên khác dùng", null, 400);

            var employeeAdded = new Employee() { Name = request.Name.Trim(), Description = request.Description, UpdateDate = DateTime.Now, GroupId = request.GroupId, Phone = request.Phone.Trim(), Email = request.Email.Trim() };
            _unitOfWork.Employees.Add(employeeAdded);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", employeeAdded);

        }

        public async Task<ResponseService> Delete(Guid id)
        {
            var employee = await _unitOfWork.Employees.Find(c => c.Id == id).Include(c => c.AdsAccounts).FirstOrDefaultAsync();
            if (employee == null)
                return new ResponseService("Not found", null);
            employee.DeleteDate = DateTime.Now;
            
            foreach (var adsAccount in employee.AdsAccounts)
                adsAccount.EmployeeId = null;
            _unitOfWork.AdsAccounts.UpdateRange(employee.AdsAccounts);
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, Guid? organizationId, Guid? branchId, Guid? groupId)
        {
            var employee = await _unitOfWork.Employees.Find(x => x.DeleteDate == null).Include(x => x.Group).ThenInclude(c => c.Branch).ThenInclude(c => c.Organization).OrderByDescending(c => c.UpdateDate).ToListAsync();
            var response = new List<Employee>();
            bool flag = false;
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);
                int skip = (pageIndex.Value - 1) * pageSize.Value;

                if (groupId != null)
                {
                    var totalCount = employee.Where(x => x.GroupId != null && x.GroupId == groupId).ToList().Count();
                    var groupQuery = employee.Where(x => x.GroupId != null && x.GroupId == groupId).ToList().Skip(skip).Take(pageSize.Value).ToList();
                    return new ResponseService("", groupQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }

                if (branchId != null)
                {
                    var totalCount = employee.Where(x => x.Group != null && x.Group.BranchId != null && x.Group.BranchId == branchId).ToList().Count();
                    var groupQuery = employee.Where(x => x.Group != null && x.Group.BranchId != null && x.Group.BranchId == branchId).ToList().Skip(skip).Take(pageSize.Value).ToList();
                    return new ResponseService("", groupQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }

                if (organizationId != null)
                {
                    var totalCount = employee.Where(x => x.Group != null && x.Group.Branch != null && x.Group.Branch.OrganizationId != null && x.Group.Branch.OrganizationId == organizationId).ToList().Count();
                    var groupQuery = employee.Where(x => x.Group != null && x.Group.Branch != null && x.Group.Branch.OrganizationId != null && x.Group.Branch.OrganizationId == organizationId).ToList().Skip(skip).Take(pageSize.Value).ToList();
                    return new ResponseService("", groupQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }
                var totalCount2 = employee.Count();
                var groupQuery2 = employee.ToList().Skip(skip).Take(pageSize.Value).ToList();
                return new ResponseService("", groupQuery2, new PagingResponse(totalCount2, pageIndex.Value, pageSize.Value));
            }

            if (organizationId != null)
            {
                response = employee.Where(x => x.Group != null && x.Group.Branch != null && x.Group.Branch.OrganizationId != null && x.Group.Branch.OrganizationId == organizationId).OrderByDescending(c => c.UpdateDate).ToList();
                flag = true;
            }
            if (branchId != null)
            {
                response = employee.Where(x => x.Group != null && x.Group.BranchId != null && x.Group.BranchId == branchId).OrderByDescending(c => c.UpdateDate).ToList();
                flag = true;
            }
            if (groupId != null)
            {
                response = employee.Where(x => x.GroupId != null && x.GroupId == groupId).OrderByDescending(c => c.UpdateDate).ToList();
                flag = true;
            }

            if (flag == true)
                return new ResponseService("", response);
            return new ResponseService("", employee);
        }

        public async Task<ResponseService> Update(UpdateEmployeeRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);

            if (request.Description.Length > 249)
                return new ResponseService("Description must < 250 character", null, 400);

            if (request.Phone.Length < 9)
                return new ResponseService("Phone not correct", null, 400);
            if (!request.Email.Contains("@"))
                return new ResponseService("Email not correct", null, 400);


            var employee = await _unitOfWork.Employees.FindOneAsync(x => x.Id == request.Id);

            if (employee == null)
                return new ResponseService("This employee not found", null, 404);

            var group = await _unitOfWork.Groups.Find(x => x.Id == request.GroupId && x.DeleteDate == null).Include(c => c.Employees).FirstOrDefaultAsync();
            if (group == null)
                return new ResponseService("Không tìm thấy đội nhóm này ", null, 404);

            if (employee.GroupId != null && employee.GroupId == request.GroupId)
            {
                var groupQuery = group.Employees.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null) && !x.Id.Equals(employee.Id)).FirstOrDefault();
                if (groupQuery != null)
                    return new ResponseService("Tên của thành viên này đã tồn tại ở đội nhóm: " + group.Name, null, 400);
            }
            else
            {
                var groupQuery = group.Employees.Where(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
                if (groupQuery != null)
                    return new ResponseService("Tên của thành viên này đã tồn tại ở đội nhóm" + group.Name, null, 400);
            }

            var employeeCHeckGmail = _unitOfWork.Employees.Find(x => (x.Email != null && x.Email.Trim().ToUpper().Equals(request.Email.Trim().ToUpper()) && x.DeleteDate == null && x.Id != request.Id)).FirstOrDefault();
            if (employeeCHeckGmail != null)
                return new ResponseService("Gmail này đã được một thành viên khác dùng", null, 400);

            var employeeCHeckPhone = _unitOfWork.Employees.Find(x => (x.Phone != null && x.Phone.Trim().ToUpper().Equals(request.Phone.Trim().ToUpper()) && x.DeleteDate == null && x.Id != request.Id)).FirstOrDefault();
            if (employeeCHeckPhone != null)
                return new ResponseService("Số điện thoại này đã được một thành viên khác dùng", null, 400);

            employee.Name = request.Name.Trim();
            employee.Description = request.Description;
            employee.UpdateDate = DateTime.Now;
            employee.GroupId = request.GroupId;
            employee.Email = request.Email.Trim();
            employee.Phone = request.Phone.Trim();
            _unitOfWork.Groups.Update(group);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", new { id = group.Id, name = group.Name, description = group.Description, updateDate = group.UpdateDate, deleteDate = group.DeleteDate, groupName = group.Name });
        }
    }
}
