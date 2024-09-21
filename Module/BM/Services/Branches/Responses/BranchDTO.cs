namespace FBAdsManager.Module.Branches.Responses
{
    public class BranchDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }

        public BranchDTO() { }

        public BranchDTO(Guid id, string name, string? description, DateTime updateDate, DateTime? deleteDate)
        {
            Id = id;
            Name = name;
            Description = description;
            UpdateDate = updateDate;
            DeleteDate = deleteDate;
        }
    }
}
