namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Databases;
using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Sieve.Models;
using Sieve.Services;


public static class GetTradingPartnerLogicalList
{
    public class TradingPartnerLogicalListQuery : IRequest<PagedList<TradingPartnerLogicalDto>>
    {
        public readonly TradingPartnerParametersDto QueryParameters;

        public TradingPartnerLogicalListQuery(TradingPartnerParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public class Handler : IRequestHandler<TradingPartnerLogicalListQuery, PagedList<TradingPartnerLogicalDto>>
    {
        private readonly TradingPartnerDbContext _dbContext;
        private readonly ITradingPartnerRepository _tradingPartnerRepository;
        private readonly SieveProcessor _sieveProcessor;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext dbContext,ITradingPartnerRepository tradingPartnerRepository, IMapper mapper, SieveProcessor sieveProcessor)
        {
            _mapper = mapper;
            _tradingPartnerRepository = tradingPartnerRepository;
            _sieveProcessor = sieveProcessor;
            _dbContext = dbContext;
        }

        public async Task<PagedList<TradingPartnerLogicalDto>> Handle(TradingPartnerLogicalListQuery request, CancellationToken cancellationToken)
        {
            var collection = _tradingPartnerRepository.Query().ToList();

            var result = collection.Select(x => new  { PostalData = x.TradingPartnerDocument?.Postal_addresses, PhoneData = x.TradingPartnerDocument?.Phone_addresses, ElectronicData = x.TradingPartnerDocument?.Electronic_addresses, DocumentData = x.TradingPartnerDocument, Id = x.Id, Name = x.Name, ShortName = x.ShortName, Num = x.Num, DunsNum = x.DunsNum, IsSubsidiary = x.IsSubsidiary, FederalEmployerIdent = x.FederalEmployerIdent, MinorityVendors = x.TradingPartnerDocument?.Minority_vendors, }).ToList().SelectMany(i => i.DocumentData.Roles.SelectMany(j => j.Statuses.Where(k => k.Thru_date == null ? k.From_date <= DateTime.Today : k.From_date <= DateTime.Today && k.Thru_date >= DateTime.Today).Select(_ => new TradingPartnerLogicalDto { Id = i.Id, Name = i.Name, ShortName = i.ShortName, Num = i.Num, Type = j.Type, Status = _.Status, DunsNum = i.DunsNum, FederalEmployerIdent = i.FederalEmployerIdent, IsSubsidiary = i.IsSubsidiary, MinorityVendors = i?.MinorityVendors, AddressType = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Type, Address_1 = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Address_1, Address_2 = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Address_2, StateType = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.State_type, City = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.City, State = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.State_name, StateCode = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.State_code, CountryName = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Country_name, CountryCode = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Country_code, PostalCode = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Postal_code, IsPostalPrimary = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Is_primary, IsDoNotContact = i?.PostalData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Is_do_not_contact, PhoneType = i?.PhoneData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Type, PhoneCountryCode = i?.PhoneData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Country_code, AreaCode = i?.PhoneData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Area_code, PhoneNumber = i?.PhoneData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Phone_number, PhoneExtension = i?.PhoneData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Phone_extension, IsPhonePrimary = i?.PhoneData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Is_primary, IsPhoneDoNotContact = i?.PhoneData?.Where(x => x.Is_primary == true).FirstOrDefault()?.Is_do_not_contact, Email = i?.ElectronicData?.Electronic_email_addresses?.Where(x => x.Is_primary == true).FirstOrDefault()?.Email, EmailType = i?.ElectronicData?.Electronic_email_addresses?.Where(x => x.Is_primary == true).FirstOrDefault()?.Type, IsEmailPrimary = i?.ElectronicData?.Electronic_email_addresses?.Where(x => x.Is_primary == true).FirstOrDefault()?.Is_primary, IsEmailDoNotContact = i?.ElectronicData?.Electronic_email_addresses?.Where(x => x.Is_primary == true).FirstOrDefault()?.Is_do_not_contact, })));

            var sieveModel = new SieveModel
            {
                Sorts = request.QueryParameters.SortOrder ?? "Id",
                Filters = request.QueryParameters.Filters
            };

            var appliedCollection = _sieveProcessor.Apply(sieveModel, result.AsQueryable());
           
            return await Task.FromResult(PagedList<TradingPartnerLogicalDto>.Create(appliedCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize, request.QueryParameters.ReturnAll));
        }
    }
}