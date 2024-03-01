using FluentValidation;
using MagicVille_CouponAPI.Models.DTO;

namespace MagicVille_CouponAPI.Validation
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}
