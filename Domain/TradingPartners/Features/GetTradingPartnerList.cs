namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Wrappers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Sieve.Models;
using Sieve.Services;

public static class GetTradingPartnerList
{
    public class TradingPartnerListQuery : IRequest<PagedList<TradingPartnerDto>>
    {
        public readonly TradingPartnerParametersDto QueryParameters;

        public TradingPartnerListQuery(TradingPartnerParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public class Handler : IRequestHandler<TradingPartnerListQuery, PagedList<TradingPartnerDto>>
    {
        private readonly ITradingPartnerRepository _tradingPartnerRepository;
        private readonly SieveProcessor _sieveProcessor;
        private readonly IMapper _mapper;

        public Handler(ITradingPartnerRepository tradingPartnerRepository, IMapper mapper, SieveProcessor sieveProcessor)
        {
            _mapper = mapper;
            _tradingPartnerRepository = tradingPartnerRepository;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PagedList<TradingPartnerDto>> Handle(TradingPartnerListQuery request, CancellationToken cancellationToken)
        {
            var collection = _tradingPartnerRepository.Query();

            var sieveModel = new SieveModel
            {
                Sorts = request.QueryParameters.SortOrder ?? "Id",
                Filters = request.QueryParameters.Filters
            };

            var appliedCollection = _sieveProcessor.Apply(sieveModel, collection);
            var dtoCollection = appliedCollection
                .ProjectTo<TradingPartnerDto>(_mapper.ConfigurationProvider);

            return await PagedList<TradingPartnerDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}