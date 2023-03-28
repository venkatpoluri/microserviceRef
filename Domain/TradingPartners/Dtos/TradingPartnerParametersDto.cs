namespace TradingPartnerManagement.Domain.TradingPartners.Dtos
{
    using SharedKernel.Dtos;

    public class TradingPartnerParametersDto : BasePaginationParameters
    {
        public string Filters { get; set; }
        public string SortOrder { get; set; }
        public bool ReturnAll { get; set; }
    }
}