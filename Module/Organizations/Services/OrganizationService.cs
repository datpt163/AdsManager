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

            var organize = _unitOfWork.Organizations.Find(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null)).FirstOrDefault();
            if (organize != null)
                return new ResponseService("Tên của hệ thống này đã tồn tại", null);
            var organizeAdd = new Common.Database.Data.Organization() { Name = request.Name.Trim(), Description = request.Description, UpdateDate = DateTime.Now };
            _unitOfWork.Organizations.Add(organizeAdd);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", organizeAdd);
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).OrderByDescending(c => c.UpdateDate).Skip(skip).Take(pageSize.Value);
                var totalCount = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Count();
                return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.Organizations.Find(x => x.DeleteDate == null).OrderByDescending(c => c.UpdateDate).ToListAsync());
        }

        public async Task<ResponseService> GetDetailAsync(Guid id)
        {
            var organization = await _unitOfWork.Organizations.FindOneAsync(c => c.Id == id);
            if (organization == null)
                return new ResponseService("Not found", null);

            return new ResponseService("", organization);
        }

        public async Task<ResponseService> Delete(Guid id)
        {
            var organization = await _unitOfWork.Organizations.Find(c => c.Id == id).Include(c => c.Users).Include(c => c.Branches).ThenInclude(c => c.Groups).ThenInclude(c => c.Employees).FirstOrDefaultAsync();
            if (organization == null)
                return new ResponseService("Not found", null);
            organization.DeleteDate = DateTime.Now;
            var error = "Phải xóa tất cả các chi nhánh thuộc hệ thống này trước khi xóa, hệ thống này hiện đang có các chi nhánh sau: ";
            foreach (var s in organization.Branches)
            {
                if (s.DeleteDate == null)
                    error += (s.Name + " ,");
            }

            if(error != "Phải xóa tất cả các chi nhánh thuộc hệ thống này trước khi xóa, hệ thống này hiện đang có các chi nhánh sau: ")
                return new ResponseService(error.Substring(0, error.Length - 2), null);
            else
            {
                if(organization.Users.Count() > 0)
                    return new ResponseService("Phải xóa tất cả trường hệ thống", null);
            }

            _unitOfWork.Organizations.Update(organization);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> Update(UpdateOrganizationRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                return new ResponseService("Name empty", null, 400);

            if (request.Description.Length > 249)
                return new ResponseService("Description must < 250 character", null);

            var organization = await _unitOfWork.Organizations.FindOneAsync(x => x.Id == request.Id);

            if (organization == null)
                return new ResponseService("This organization not found", null, 404);

            var organize = _unitOfWork.Organizations.Find(x => (x.Name.Trim().ToUpper().Equals(request.Name.Trim().ToUpper()) && x.DeleteDate == null) && !x.Name.Equals(organization.Name)).FirstOrDefault();
            if (organize != null)
                return new ResponseService("Tên của hệ thống này đã tồn tại", null);

            organization.Name = request.Name.Trim();
            organization.Description = request.Description;
            organization.UpdateDate = DateTime.Now;
            _unitOfWork.Organizations.Update(organization);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", organization);
        }
    }
}
