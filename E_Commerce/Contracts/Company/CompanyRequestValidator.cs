namespace E_Commerce.Contracts.Company
{
    public class CompanyRequestValidator :AbstractValidator<CompanyRequest>
    {
        public CompanyRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Description).MinimumLength(3).MaximumLength(150);
            RuleFor(x => x.Address).MinimumLength(3).MaximumLength(150);
        }
    }
}
