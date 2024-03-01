﻿using MagicVille_CouponAPI.Models;

namespace MagicVille_CouponAPI.Data
{
    public static class CouponStore
    {
        public static List<Coupon> CouponList = new()
        {
            new Coupon{Id=1,Name="10OFF",Percent=10,IsActive=true},
            new Coupon{Id=2,Name="20OFF",Percent=20,IsActive=true},
            new Coupon{Id=3,Name="30OFF",Percent=30,IsActive=true}
        };
    }
}
