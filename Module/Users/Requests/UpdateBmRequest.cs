﻿namespace FBAdsManager.Module.Users.Requests
{
    public class UpdateBmRequest
    {
        public Guid Id { get; set; }
        public string email { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string ChatId { get; set; } = string.Empty;
        public List<BmInformation> Bms { get; set; } = new List<BmInformation>();
    }
}
