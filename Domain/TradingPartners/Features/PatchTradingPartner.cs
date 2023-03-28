namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using SharedKernel.Exceptions;
using TradingPartnerManagement.Domain.TradingPartners.Services;
using TradingPartnerManagement.Services;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;

public static class PatchTradingPartner
{
    public class PatchTradingPartnerCommand : IRequest<bool>
    {
        public readonly Guid Id;
        public readonly JsonPatchDocument<TradingPartnerForUpdateDto> PatchDoc;

        public PatchTradingPartnerCommand(Guid tradingPartner, JsonPatchDocument<TradingPartnerForUpdateDto> patchDoc)
        {
            Id = tradingPartner;
            PatchDoc = patchDoc;
        }
    }

    public class Handler : IRequestHandler<PatchTradingPartnerCommand, bool>
    {
        private readonly ITradingPartnerRepository _tradingPartnerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public Handler(ITradingPartnerRepository tradingPartnerRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _tradingPartnerRepository = tradingPartnerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(PatchTradingPartnerCommand request, CancellationToken cancellationToken)
        {
            if (request.PatchDoc == null)
                throw new ValidationException(
                    new List<ValidationFailure>()
                    {
                        new ValidationFailure("Patch Document","Invalid patch doc.")
                    });

            var tradingPartnerToUpdate = await _tradingPartnerRepository.GetById(request.Id, cancellationToken: cancellationToken);

            var tradingPartnerToPatch = _mapper.Map<TradingPartnerForUpdateDto>(tradingPartnerToUpdate);
            request.PatchDoc.ApplyTo(tradingPartnerToPatch);

            tradingPartnerToUpdate.Update(tradingPartnerToPatch);
            await _unitOfWork.CommitChanges(cancellationToken);

            return true;
        }
    }
}