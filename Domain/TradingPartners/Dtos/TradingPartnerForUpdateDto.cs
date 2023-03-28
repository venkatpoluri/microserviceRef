namespace TradingPartnerManagement.Domain.TradingPartners.Dtos
{
    using System.Collections.Generic;
    using System;

    public class TradingPartnerForUpdateDto : TradingPartnerForManipulationDto
    {
    
        public TradingPartnerForUpdateDto(){}

        public TradingPartnerForUpdateDto(TradingPartner entity)
        {
            if(entity != null) 
            {
             Id = entity.Id;
               Name = entity.Name;
   ShortName = entity.ShortName;
   DunsNum = entity.DunsNum;
   IsSubsidiary = entity.IsSubsidiary;
   ExternalOid = entity.ExternalOid;
   ParentExternalOid = entity.ParentExternalOid;
   FederalEmployerIdent = entity.FederalEmployerIdent;
   ConceptId = entity.ConceptId;
   ConceptKey = entity.ConceptKey;
   TradingPartnerDocument = new TradingPartnerDocument {
           Roles = entity.TradingPartnerDocument.Roles,
           Postal_addresses = entity.TradingPartnerDocument.Postal_addresses,
           Phone_addresses = entity.TradingPartnerDocument.Phone_addresses,
           Electronic_addresses = entity.TradingPartnerDocument.Electronic_addresses,
           Notes = entity.TradingPartnerDocument.Notes,
           Trading_partner_properties = entity.TradingPartnerDocument.Trading_partner_properties,
           Minority_vendors = entity.TradingPartnerDocument.Minority_vendors
};

            }           
        }

        public Guid Id { get; set; }
        

    }
}