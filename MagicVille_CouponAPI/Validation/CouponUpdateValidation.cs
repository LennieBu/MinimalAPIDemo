using FluentValidation;
using MagicVille_CouponAPI.Data;
using MagicVille_CouponAPI.Models.DTO;

namespace MagicVille_CouponAPI.Validation
{
    public class CouponUpdateValidation : AbstractValidator<CouponUpdateDTO>
    {
        public CouponUpdateValidation()
        {
            RuleFor(model => model.Id).GreaterThan(0);
            RuleFor(model => model.Id).LessThanOrEqualTo(CouponStore.CouponList.Count);
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Percent).InclusiveBetween(1, 100);
        }
    }
}

