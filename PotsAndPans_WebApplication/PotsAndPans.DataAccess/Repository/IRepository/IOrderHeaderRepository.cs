using PotsAndPans.Models;
using PotsAndPans.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PotsAndPans.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader> 
    {
        void Update(OrderHeader obj);
        void UpdateStatus(int id, string OrderStatus, string? paymentStatus=null);
        void UpdatedStripePaymentID(int id, string sessionId, string paymentIntentId);

    }
}
