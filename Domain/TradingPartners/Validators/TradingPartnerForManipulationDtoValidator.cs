namespace TradingPartnerManagement.Domain.TradingPartners.Validators;

using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using FluentValidation;
using TradingPartnerManagement.Databases;
using Microsoft.EntityFrameworkCore;

public class TradingPartnerForManipulationDtoValidator<T> : AbstractValidator<T> where T : TradingPartnerForManipulationDto
{
    private readonly TradingPartnerDbContext _context;
    public TradingPartnerForManipulationDtoValidator(TradingPartnerDbContext context)
    {
        _context = context;
        // add fluent validation rules that should be shared between creation and update operations here
        //https://fluentvalidation.net/
                 RuleFor(x=> x.Name).NotEmpty().WithMessage("Name is required.");

         RuleFor(x=> x.ShortName).NotEmpty().WithMessage("ShortName is required.");

         RuleFor(x=> x.ConceptId).NotEmpty().WithMessage("ConceptId is required.");

        RuleFor(x => x.ConceptId).Must(BeValidConceptId).WithMessage("The specified ConceptId not exists.");

         RuleFor(x=> x.ConceptKey).NotEmpty().WithMessage("ConceptKey is required.");


        
        When(x => x.TradingPartnerDocument?.Postal_addresses != null, () =>
         {
            RuleFor(x => x.TradingPartnerDocument)
            .Must(y => y.Postal_addresses.Where(z => z.Is_primary == true).Count() == 1)
            .WithErrorCode("ERRTPFA1PG")
            .WithMessage("TradingPartner Postal Address must have only one Is Primary flag assigned.");

        });

        When(x => x.TradingPartnerDocument?.Phone_addresses != null, () =>
         {
            RuleFor(x => x.TradingPartnerDocument)
            .Must(y => y.Phone_addresses.Where(z => z.Is_primary == true).Count() == 1)
            .WithErrorCode("ERRTP1PHPG")
            .WithMessage("TradingPartner Phone Address must have only one Is Primary flag assigned.");

        });

        When(x => x.TradingPartnerDocument?.Electronic_addresses != null, () =>
         {
            RuleFor(x => x.TradingPartnerDocument)
            .Must(y => (y.Electronic_addresses.Electronic_email_addresses != null && y.Electronic_addresses.Electronic_website_addresses != null) ? (y.Electronic_addresses.Electronic_email_addresses.Where(x => x.Is_primary == true).Count() + y.Electronic_addresses.Electronic_website_addresses.Where(x => x.Is_primary == true).Count()) == 1 : y.Electronic_addresses.Electronic_email_addresses != null ? y.Electronic_addresses.Electronic_email_addresses.Where(x => x.Is_primary == true).Count() == 1 : y.Electronic_addresses.Electronic_website_addresses != null ? y.Electronic_addresses.Electronic_website_addresses.Where(x => x.Is_primary == true).Count() == 1 : false)
            .WithErrorCode("ERRTPEA1PG")
            .WithMessage("TradingPartner Electronic Address must have only one Is Primary flag assigned.");

        });

        When(x => x.TradingPartnerDocument?.Roles != null, () =>
         {
            RuleFor(x => x.TradingPartnerDocument)
            .Must(x => x.Roles.All(y => y.Statuses.Count() > 0))
            .WithErrorCode("ERRTPFA1US")
            .WithMessage("TradingPartner Role must have at least one Status record.");

        });

        When(x => x.TradingPartnerDocument?.Roles != null, () =>
         {
            RuleFor(x => x.TradingPartnerDocument)
            .Must(x => x.Roles.Where(z=>z.Type == 0).Count() < 1)
            .WithErrorCode("ERRTPFA1RL")
            .WithMessage("TradingPartner must have at least one Role Type assigned.");

        });

        When(x => x.TradingPartnerDocument?.Roles != null, () =>
         {
            RuleFor(x => x.TradingPartnerDocument)
            .Must(x => x.Roles.All(y => y.Statuses.All(status => !y.Statuses.Where(z => !ReferenceEquals(status,z)).Any(z => z.From_date <= status.Thru_date && z.Thru_date >= status.From_date))))
            .WithErrorCode("ERRTPFAEDO")
            .WithMessage("TradingPartner Role Status effective dates cannot overlap any existing effective dates.");

        });

        When(x => x.TradingPartnerDocument?.Roles != null, () =>
         {
            RuleFor(x => x.TradingPartnerDocument)
            .Must(x => x.Roles.All(y => y.Statuses.Where(z=> z.Thru_date == null || !z.Thru_date.HasValue).Count() == 1))
            .WithErrorCode("ERRTPFABED")
            .WithMessage("TradingPartner Role must have only one Trading Role Status with a blank End Date.");

        });

    }

    // want to do some kind of db check to see if something is unique? try something like this with the `MustAsync` prop
    // source: https://github.com/jasontaylordev/CleanArchitecture/blob/413fb3a68a0467359967789e347507d7e84c48d4/src/Application/TodoLists/Commands/CreateTodoList/CreateTodoListCommandValidator.cs
    // public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    // {
    //     return await _context.TodoLists
    //         .AllAsync(l => l.Title != title, cancellationToken);
    // }
            public bool BeValidConceptId(Guid param)=>  _context.Concepts
                                .Any(l => l.Id == param);


}