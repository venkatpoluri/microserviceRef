namespace TradingPartnerManagement.Domain.TradingPartners.Validators;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using FluentValidation;
using TradingPartnerManagement.Databases;
using Microsoft.EntityFrameworkCore;


public class TradingPartnerForUpdateDtoValidator: TradingPartnerForManipulationDtoValidator<TradingPartnerForUpdateDto>
{
    private readonly TradingPartnerDbContext _context;

    public TradingPartnerForUpdateDtoValidator(TradingPartnerDbContext context): base(context)
    {
       _context = context;
        // add fluent validation rules that should be shared between creation and update operations here
        //https://fluentvalidation.net/
            RuleFor(x => x.Name).Must(BeUniqueName).WithMessage("The specified Name already exists.");

        RuleFor(x => x.ShortName).Must(BeUniqueShortName).WithMessage("The specified ShortName already exists.");


    
    }

            public bool BeUniqueName(TradingPartnerForUpdateDto entity ,string title)=>  _context.TradingPartners
                                .Where(l => l.Id != entity.Id).All(l => l.Name != title);

        public bool BeUniqueShortName(TradingPartnerForUpdateDto entity ,string title)=>  _context.TradingPartners
                                .Where(l => l.Id != entity.Id).All(l => l.ShortName != title);


}