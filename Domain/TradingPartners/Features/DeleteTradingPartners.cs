namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Services;
using TradingPartnerManagement.Databases;
using TradingPartnerManagement.Domain.TradingPartners.Validators;
using MediatR;
using FluentValidation;

public static class DeleteTradingPartners
{
    public class DeleteTradingPartnersCommand : IRequest<bool>
    {
        public readonly IEnumerable<Guid> Ids;

        public DeleteTradingPartnersCommand(IEnumerable<Guid> tradingPartners)
        {
            Ids = tradingPartners;
        }
    }

    public class Handler : IRequestHandler<DeleteTradingPartnersCommand, bool>
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

        public async Task<bool> Handle(DeleteTradingPartnersCommand request, CancellationToken cancellationToken)
        {
            var results = request.Ids.Select(id => new { id, validId = _tradingPartnerRepository.Exists(id).GetAwaiter().GetResult()}).ToList();
            var tradingPartnersToDelete = results.Where(x => x.validId).Select(x => _tradingPartnerRepository.GetById(x.id).GetAwaiter().GetResult());

            _tradingPartnerRepository.RemoveRange(tradingPartnersToDelete);
            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}