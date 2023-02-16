using PotsAndPans.Models.Models;

namespace PotsAndPans.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
       
    }
}
