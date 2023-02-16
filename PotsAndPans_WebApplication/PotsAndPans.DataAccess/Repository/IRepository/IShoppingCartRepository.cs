﻿using PotsAndPans.Models;
using PotsAndPans.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart> 
    {
        int IncrementCount(ShoppingCart shoppingCart, int count);
        int DecrementCount(ShoppingCart shoppingCart, int count);

    }
}
