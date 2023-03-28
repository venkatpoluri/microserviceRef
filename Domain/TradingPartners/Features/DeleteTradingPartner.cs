namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Services;
using TradingPartnerManagement.Databases;
using TradingPartnerManagement.Domain.TradingPartners.Validators;
using MediatR;
using FluentValidation;

public static class DeleteTradingPartner
{
    public class DeleteTradingPartnerCommand : IRequest<bool>
    {
        public readonly Guid Id;

        public DeleteTradingPartnerCommand(Guid tradingPartner)
        {
            Id = tradingPartner;
        }
    }

    public class Handler : IRequestHandler<DeleteTradingPartnerCommand, bool>
    {
        private readonly ITradingPartnerRepository _tradingPartnerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TradingPartnerDbContext _dbContext;

        public Handler(ITradingPartnerRepository tradingPartnerRepository, IUnitOfWork unitOfWork,TradingPartnerDbContext dbContext)
        {
            _tradingPartnerRepository = tradingPartnerRepository;
            _unitOfWork = unitOfWork;
            _dbContext =dbContext;
        }

        public async Task<bool> Handle(DeleteTradingPartnerCommand request, CancellationToken cancellationToken)
        {
            new TradingPartnerForDeleteDtoValidator(_dbContext).ValidateAndThrow(request.Id);

            var recordToDelete = await _tradingPartnerRepository.GetById(request.Id, cancellationToken: cancellationToken);

            _tradingPartnerRepository.Remove(recordToDelete);
            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}