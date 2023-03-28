namespace TradingPartnerManagement.Domain.TradingPartners.Features;

using SharedKernel.Exceptions;
using TradingPartnerManagement.Databases;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Domain.Contacts.Dtos;

public static class ReadTradingPartnerListByContactId
{
    public class ReadTradingPartnerListByContactIdQuery : IRequest<List<TradingPartnerDto>>
    {
        public Guid Id { get; set; }
        public AlignmentsType Type { get; set; }


        public ReadTradingPartnerListByContactIdQuery(Guid id, AlignmentsType type)
        {
            Id = id;
            Type = type;

        }
    }

    public class Handler : IRequestHandler<ReadTradingPartnerListByContactIdQuery, List<TradingPartnerDto>>
    {
        private readonly TradingPartnerDbContext _db;
        private readonly IMapper _mapper;

        public Handler(TradingPartnerDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<List<TradingPartnerDto>> Handle(ReadTradingPartnerListByContactIdQuery request, CancellationToken cancellationToken)
        {
            var ContactListQuery = await _db.Contacts.Where(x => ((x.Id == request.Id))).FirstOrDefaultAsync();
            if (ContactListQuery == null)
                throw new NotFoundException("Contact");

            if (ContactListQuery.ContactDocument == null)
                throw new NotFoundException("ContactDocument");

            if (ContactListQuery.ContactDocument.Alignments.Count ()== 0)
                throw new NotFoundException("ContactDocument");

            var contactDocument = ContactListQuery.ContactDocument.Alignments.Where(x => x.Type == request.Type);
            var tradingPartnerList = new List<TradingPartner>();
            var tradingpartnerIds = contactDocument.Select(x => x.Id);
            foreach (var tradingpartnerId in tradingpartnerIds)
            {
                var partner = await _db.TradingPartners.Where(x => x.Id == tradingpartnerId).FirstOrDefaultAsync();
                tradingPartnerList.Add(partner);
            }

            return _mapper.Map<List<TradingPartnerDto>>(tradingPartnerList);
        }
    }
}