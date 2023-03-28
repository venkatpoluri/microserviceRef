namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Domain.TradingPartners;
using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Services;
using TradingPartnerManagement.Domain.TradingPartners.Validators;
using AutoMapper;
using MediatR;
using TradingPartnerManagement.Databases;
using FluentValidation;

public static class AddTradingPartner
{
    public class AddTradingPartnerCommand : IRequest<TradingPartnerDto>
    {
        public readonly TradingPartnerForCreationDto TradingPartnerToAdd;

        public AddTradingPartnerCommand(TradingPartnerForCreationDto tradingPartnerToAdd)
        {
            TradingPartnerToAdd = tradingPartnerToAdd;
        }
    }

    public class Handler : IRequestHandler<AddTradingPartnerCommand, TradingPartnerDto>
    {
        private readonly ITradingPartnerRepository _tradingPartnerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; 
        private readonly TradingPartnerDbContext _dbContext;


        public Handler(ITradingPartnerRepository tradingPartnerRepository, IUnitOfWork unitOfWork, IMapper mapper,TradingPartnerDbContext dbContext)
        {
            _mapper = mapper;
            _tradingPartnerRepository = tradingPartnerRepository;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<TradingPartnerDto> Handle(AddTradingPartnerCommand request, CancellationToken cancellationToken)
        {
            new TradingPartnerForCreationDtoValidator(_dbContext).ValidateAndThrow(request.TradingPartnerToAdd);  
          
            var tradingPartner = TradingPartner.Create(request.TradingPartnerToAdd);
            await _tradingPartnerRepository.Add(tradingPartner, cancellationToken);

            await _unitOfWork.CommitChanges(cancellationToken);

            var tradingPartnerAdded = await _tradingPartnerRepository.GetById(tradingPartner.Id, cancellationToken: cancellationToken);
            return _mapper.Map<TradingPartnerDto>(tradingPartnerAdded);
        }
    }
}