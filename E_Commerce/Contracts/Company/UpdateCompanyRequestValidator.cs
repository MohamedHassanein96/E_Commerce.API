namespace E_Commerce.Contracts.Company
{
    public class UpdateCompanyRequestValidator :AbstractValidator<UpdateCompanyRequest>
    {
        public UpdateCompanyRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.Address).MinimumLength(3).MaximumLength(150); 
        }
    }
}
