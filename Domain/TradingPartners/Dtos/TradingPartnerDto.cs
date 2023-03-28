namespace TradingPartnerManagement.Domain.TradingPartners.Dtos
{
    using System.Collections.Generic;
    using System;

    public class TradingPartnerDto 
    {
    
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string? DunsNum { get; set; }
        public bool? IsSubsidiary { get; set; }
        public Guid? ExternalOid { get; set; }
        public Guid? ParentExternalOid { get; set; }
        public int Num { get; set; }
        public string? FederalEmployerIdent { get; set; }
        public Guid ConceptId { get; set; }
        public string ConceptKey { get; set; }
        public TradingPartnerDocument TradingPartnerDocument { get; set; }
    }
}