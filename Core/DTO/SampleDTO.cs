using Jackobo.WebApiTemplate.Core.Enums;

namespace Jackobo.WebApiTemplate.Core.DTO
{
    /// <summary>
    /// Sample data transfer object returned by the API.
    /// DTOs are the contract exposed to clients — keep them free of persistence concerns.
    ///
    /// TODO: Replace this with the DTOs your endpoints return.
    /// </summary>
    public class SampleDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public SampleStatusEnum Status { get; set; } = SampleStatusEnum.Active;

        public DateTime CreatedOnUtc { get; set; }
    }
}
