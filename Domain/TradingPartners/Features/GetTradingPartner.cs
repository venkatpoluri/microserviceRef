namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using AutoMapper;
using MediatR;

public static class GetTradingPartner
{
    public class TradingPartnerQuery : IRequest<TradingPartnerDto>
    {
        public readonly Guid Id;

        public TradingPartnerQuery(Guid id)
        {
            Id = id;
        }
    }

    public class Handler : IRequestHandler<TradingPartnerQuery, TradingPartnerDto>
    {
        private readonly ITradingPartnerRepository _tradingPartnerRepository;
        private readonly IMapper _mapper;

        public Handler(ITradingPartnerRepository tradingPartnerRepository, IMapper mapper)
        {
            _mapper = mapper;
            _tradingPartnerRepository = tradingPartnerRepository;
        }

        public async Task<TradingPartnerDto> Handle(TradingPartnerQuery request, CancellationToken cancellationToken)
        {
            var result = await _tradingPartnerRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return _mapper.Map<TradingPartnerDto>(result);
        }
    }
}