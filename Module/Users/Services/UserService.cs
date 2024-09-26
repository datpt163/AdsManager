using FBAdsManager.Common.Database.Data;
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
                return new ResponseService("Email something wrong", null, 400);

            var role = await _unitOfWork.Roles.FindOneAsync(x => x.Id == request.RoleId);
            if (role == null)
                return new ResponseService("Role not found", null, 404);

            if (!role.Name.Equals("PM"))
            {
                if (request.Password == null || request.Password.Length < 6)
                    return new ResponseService("Pass must >= 6 character", null, 400);

                if (role.Name.Equals("GROUP"))
                {
                    var group = _unitOfWork.Groups.FindOne(x => x.Id == (request.GroupId == null ? Guid.NewGuid() : request.GroupId.Value));
                    if(group == null)
                        return new ResponseService("Group not found", null, 404);
                }

                if (role.Name.Equals("BRANCH"))
                {
                    var branch = _unitOfWork.Branchs.FindOne(x => x.Id == (request.BranchId == null ? Guid.NewGuid() : request.BranchId.Value));
                    if (branch == null)
                        return new ResponseService("Branch not found", null, 404);
                }

                if (role.Name.Equals("ORGANIZATION"))
                {
                    var organization = _unitOfWork.Organizations.FindOne(x => x.Id == (request.OrganizationId == null ? Guid.NewGuid() : request.OrganizationId.Value));
                    if (organization == null)
                        return new ResponseService("organization not found", null, 404);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(request.Password))
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

            var User = _unitOfWork.Users.Find(x => (x.Email.Trim().Equals(request.Email.Trim()) && x.IsActive == true) && x.Role.Name != "BM").FirstOrDefault();
            if (User != null)
                return new ResponseService("Email này đã được sử dụng ở một tài khoản khác", null);
            var userAdd = new Common.Database.Data.User() { Email = request.Email.Trim(), Password = request.Password, GroupId = request.GroupId, IsActive = true, RoleId = request.RoleId, OrganizationId = request.OrganizationId, BranchId = request.BranchId };
            _unitOfWork.Users.Add(userAdd);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", userAdd);
        }

        public async Task<ResponseService> GetListAsyncSystem(int? pageIndex, int? pageSize, Guid? roleId)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedUserQuery = _unitOfWork.Users.Find(x => x.IsActive == true && x.Role.Name != "BM").Include(c => c.Group).Include(c => c.Role).Skip(skip).Take(pageSize.Value);
                if (roleId.HasValue)
                    pagedUserQuery = pagedUserQuery.Where(x => x.RoleId == roleId.Value);
                var totalCount = _unitOfWork.Users.Find(x => x.IsActive == true && !x.Role.Name.Equals("BM")).Count();
                return new ResponseService("", pagedUserQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.Users.Find(x => x.IsActive == true && x.Role.Name != "BM").Include(c => c.Group).Include(c => c.Role).ToListAsync());
        }

        public async Task<ResponseService> GetListAsyncBm(int? pageIndex, int? pageSize)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedUserQuery = _unitOfWork.Users.Find(x => x.IsActive == true && x.Role.Name.Equals("BM")).Include(c => c.Group).ThenInclude(c => c.Branch).ThenInclude(c => c.Organization).Include(c => c.Pms).Skip(skip).Take(pageSize.Value).Select(x => new
                {
                    Id = x.Id,
                    Email = x.Email,
                    Group = x.Group,
                    TokenTelegram = x.TokenTelegram,
                    ChatId = x.ChatId,
                    Pms = x.Pms,
                });
                var totalCount = _unitOfWork.Users.Find(x => x.IsActive == true && x.Role.Name.Equals("BM")).Count();
                return new ResponseService("", pagedUserQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.Users.Find(x => x.IsActive == true && x.Role.Name.Equals("BM")).Include(c => c.Group).ThenInclude(c => c.Branch).ThenInclude(c => c.Organization).Include(c => c.Role).Select(x => new
            {
                Id = x.Id,
                Email = x.Email,
                Group = x.Group,
                TokenTelegram = x.TokenTelegram,
                ChatId = x.ChatId,
                Pms = x.Pms,
            }).ToListAsync());
        }

        public async Task<ResponseService> CreateAsyncBm(CreateBmRequest request)
        {
            if (!request.email.Contains("@"))
                return new ResponseService("Must enter email", null, 400);
            if (!request.email.Contains("@"))
                return new ResponseService("Must enter email", null, 400);
            if (string.IsNullOrEmpty(request.TokenTelegram))
                return new ResponseService("Token Telegram wrong", null, 400);
            if (string.IsNullOrEmpty(request.ChatId))
                return new ResponseService("Chat id empty", null, 400);

            var userCheckEmail = _unitOfWork.Users.FindOne(x => x.Email.Trim().ToUpper().Equals(request.email.Trim().ToUpper()) && x.Role.Name.Equals("BM"));
            if (userCheckEmail != null)
                return new ResponseService("Email này đã được sử dụng cho một Bm khác", null, 400);
            var group = _unitOfWork.Groups.Find(x => x.Id == request.GroupId).Include(c => c.Users).ThenInclude(c => c.Role).FirstOrDefault();
            if (group == null)
                return new ResponseService("Không tìm thấy đội nhóm", null, 404);

            var role = _unitOfWork.Roles.FindOne(x => x.Name == "BM");

            var user = new User() { GroupId = request.GroupId, Email = request.email, RoleId = role.Id, IsActive = true, TokenTelegram = request.TokenTelegram, ChatId = request.ChatId };

            foreach (var l in request.BmsId)
            {
                var bm = _unitOfWork.Pms.FindOne(x => x.Id == l);
                if (bm == null)
                    user.Pms.Add(new Pm() { Id = l, UserId = user.Id });
                else
                {
                    var response = await Delete(user.Id);
                    return new ResponseService("Phát hiện tài khoản BM đã được sử dụng bởi một tài khoản khác", null, 404);
                }
            }
            _unitOfWork.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> Delete(Guid id)
        {
            var user = await _unitOfWork.Users.Find(c => c.Id == id).Include(c => c.Pms).ThenInclude(c => c.AdsAccounts).FirstOrDefaultAsync();
            if (user == null)
                return new ResponseService("Not found", null);

            foreach (var b in user.Pms)
            {
                b.AdsAccounts = new List<FBAdsManager.Common.Database.Data.AdsAccount>();
                _unitOfWork.Pms.Update(b);
                _unitOfWork.Pms.Remove(b);
            }

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> UpdateBmAsync(UpdateBmRequest request)
        {
            if (!request.email.Contains("@"))
                return new ResponseService("Must enter email", null, 400);
            if (request.BmsId.Count == 0)
                return new ResponseService("Must enter bm id", null, 400);
            if (string.IsNullOrEmpty(request.TokenTelegram))
                  return new ResponseService("Token telegram empty", null, 400);
            if (string.IsNullOrEmpty(request.ChatId))
                return new ResponseService("chat id empty", null, 400);

            var userCheckEmail = _unitOfWork.Users.FindOne(x => x.Email.Trim().ToUpper().Equals(request.email.Trim().ToUpper()) && x.Role.Name.Equals("BM") && x.Id != request.Id);
            if (userCheckEmail != null)
                return new ResponseService("Email này đã được sử dụng cho một Bm khác", null, 400);
            var group = _unitOfWork.Groups.Find(x => x.Id == request.GroupId).Include(c => c.Users).ThenInclude(c => c.Role).FirstOrDefault();
            if (group == null)
                return new ResponseService("Không tìm thấy đội nhóm", null, 404);

            var role = _unitOfWork.Roles.FindOne(x => x.Name == "BM");

            var pms = _unitOfWork.Pms.Find(x => x.UserId == request.Id);
            if (pms != null)
                _unitOfWork.Pms.RemoveRange(pms);

            var bm = _unitOfWork.Users.Find(x => x.Id == request.Id).Include(c => c.Pms).FirstOrDefault();
            if (bm == null)
                return new ResponseService("Không tìm thấy user này ", null, 404);

            bm.GroupId = request.GroupId;
            bm.Email = request.email;
            bm.TokenTelegram = request.TokenTelegram;
            bm.ChatId = request.ChatId;
            foreach (var l in request.BmsId)
            {
                var bm2 = _unitOfWork.Pms.FindOne(x => x.Id == l && x.UserId != request.Id);
                if (bm2 == null)
                    _unitOfWork.Pms.Add(new Pm() { Id = l, UserId = bm.Id });
                else
                {
                    return new ResponseService("Phát hiện Id BM đã được sử dụng bởi một tài khoản khác", null, 404);
                }
            }
            _unitOfWork.Users.Update(bm);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> UpdateSystemAsync(UpdateUserSystemRequest request)
        {
            var user = _unitOfWork.Users.Find(x => x.Id == request.Id).Include(c => c.Role).Include(c => c.Organization).Include(c => c.Branch).Include(c => c.Group).FirstOrDefault();
            if (user == null)
                return new ResponseService("User not found", null, 404);
            var role = _unitOfWork.Roles.FindOne(x => x.Id == request.RoleId);
            if (role == null)
                return new ResponseService("Role not found", null, 404);
            if(!request.Email.Contains("@"))
                return new ResponseService("Email not correct", null, 400);
            if(request.Password.Length <6 )
                return new ResponseService("Password must >= 6", null, 400);

            var userCheckEmail = _unitOfWork.Users.FindOne(x => (x.Email.Trim().Equals(request.Email.Trim()) && x.IsActive == true) && x.Role.Name != "BM" && x.Id != request.Id);
            if(userCheckEmail != null)
                return new ResponseService("Tài khoản email này đã được sử dụng bởi một tài khoản khác", null, 400);
            if (user.Role.Name.Equals("ORGANIZATION"))
            {
                var organization = _unitOfWork.Organizations.FindOne(x => x.Id == (request.OrganizationId == null ? Guid.NewGuid() : request.OrganizationId.Value));
                if(organization == null)
                    return new ResponseService("Không tìm thấy tổ chức này", null, 400);
            }

            if (user.Role.Name.Equals("BRANCH"))
            {
                var branch = _unitOfWork.Branchs.FindOne(x => x.Id == (request.BranchId == null ? Guid.NewGuid() : request.BranchId.Value));
                if (branch == null)
                    return new ResponseService("Không tìm thấy chi nhánh này", null, 400);
            }

            if (user.Role.Name.Equals("GROUP"))
            {
                var group = _unitOfWork.Groups.FindOne(x => x.Id == (request.GroupId == null ? Guid.NewGuid() : request.GroupId.Value));
                if (group == null)
                    return new ResponseService("Không tìm thấy đội nhóm này", null, 400);
            }


            user.Email = request.Email;
            user.Password = request.Password;
            user.RoleId = request.RoleId;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", user);
        }
    }
}
