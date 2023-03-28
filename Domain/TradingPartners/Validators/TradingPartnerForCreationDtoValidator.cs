namespace TradingPartnerManagement.Domain.TradingPartners.Validators;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using FluentValidation;
using TradingPartnerManagement.Databases;
using Microsoft.EntityFrameworkCore;

public class TradingPartnerForCreationDtoValidator: TradingPartnerForManipulationDtoValidator<TradingPartnerForCreationDto>
{
    private readonly TradingPartnerDbContext _context;

    public TradingPartnerForCreationDtoValidator(TradingPartnerDbContext context): base(context)
    {
        _context = context;
        // add fluent validation rules that should be shared between creation and update operations here
        //https://fluentvalidation.net/
                RuleFor(x => x.Name).Must(BeUniqueName).WithMessage("The specified Name already exists.");

        RuleFor(x => x.ShortName).Must(BeUniqueShortName).WithMessage("The specified ShortName already exists.");


        
    }
                public bool BeUniqueName(string title)=>  _context.TradingPartners
                                    .All(l => l.Name != title);

        public bool BeUniqueShortName(string title)=>  _context.TradingPartners
                                    .All(l => l.ShortName != title);


}