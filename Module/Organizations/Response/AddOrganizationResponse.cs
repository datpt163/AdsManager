namespace FBAdsManager.Module.Organizations.Response
{
    public class AddOrganizationResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool isDelete { get; set; }

        public AddOrganizationResponse(Guid id, string name, string? description, DateTime updateDate)
        {
            Id = id;
            Name = name;
            Description = description;
            UpdateDate = updateDate;
        }
    }
}
