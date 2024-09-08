using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Users.Requests;
using Microsoft.EntityFrameworkCore;

namespace FBAdsManager.Module.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> AddAsync(AddUserRequest request)
        {
            if (!request.Email.Contains("@"))
                return new ResponseService("Email something wrong", null,400);
          
            var role = await _unitOfWork.Roles.FindOneAsync(x => x.Id == request.RoleId);
            if (role == null)
                return new ResponseService("Role not found", null,404);

            if (!role.Name.Equals("PM"))
            {
                if(request.Password == null || request.Password.Length < 6)
                    return new ResponseService("Pass must >= 6 character", null, 400);
                if (request.GroupId != null)
                {
                     return new ResponseService("Just PM have role", null, 404);
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(request.Password) )
                    return new ResponseService("PM not have password", null, 404);
                if (request.GroupId != null)
                {
                    var group = await _unitOfWork.Groups.FindOneAsync(x => x.Id == request.GroupId);
                    if (group == null)
                        return new ResponseService("Group not found", null, 404);
                }
                else
                {
                    return new ResponseService("PM must have group", null, 404);
                }
            }
            var User = _unitOfWork.Users.Find(x => (x.Email.Trim().ToUpper().Equals(request.Email.Trim().ToUpper()) && x.IsActive == true)).FirstOrDefault();
            if (User != null)
                return new ResponseService("Email này đã được sử dụng ở một tài khoản khác", null);
            var userAdd = new Common.Database.Data.User() { Email = request.Email.Trim(), Password = request.Password, GroupId = request.GroupId, IsActive = true, RoleId = request.RoleId };
            _unitOfWork.Users.Add(userAdd);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", userAdd);
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedUserQuery = _unitOfWork.Users.Find(x => x.IsActive == true).Include(c => c.Group).Include(c => c.Role).Skip(skip).Take(pageSize.Value);
                var totalCount = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Count();
                return new ResponseService("", pagedUserQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.Users.Find(x => x.IsActive == true).Include(c => c.Group).Include(c => c.Role).ToListAsync());
        }
    }
}
