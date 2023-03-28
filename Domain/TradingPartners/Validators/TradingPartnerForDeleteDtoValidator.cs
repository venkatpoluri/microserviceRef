namespace TradingPartnerManagement.Domain.TradingPartners.Validators;

using FluentValidation;
using TradingPartnerManagement.Databases;
using Microsoft.EntityFrameworkCore;

public class TradingPartnerForDeleteDtoValidator : AbstractValidator<Guid>
{
    private readonly TradingPartnerDbContext _context;

    public TradingPartnerForDeleteDtoValidator(TradingPartnerDbContext context)
    {
        _context = context;
        // add fluent validation rules that should be shared between creation and update operations here
        //https://fluentvalidation.net/
       
    }
       
}