using PotsAndPans.DataAccess.Repository.IRepository;
using PotsAndPans.Models.Models;
using PotsAndPans.Models.Models.ViewModels;
using PotsAndPans.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace PotsAndPans.Areas.Customer.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderManagementController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]

        public OrderManagementVM OrderManagementVM { get; set; }
        public OrderManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderManagementVM = new OrderManagementVM
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),

            };
            return View(OrderManagementVM);
        }

        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPayNow()
        {

            OrderManagementVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderManagementVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderManagementVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == OrderManagementVM.OrderHeader.Id, includeProperties: "Product");

            //Strip Settings
            var domain = "https://localhost:7001/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/OrderManagement/PaymentConfirmation?orderHeaderid={OrderManagementVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/OrderManagement/details?orderId={OrderManagementVM.OrderHeader.Id}",
            };

            foreach (var item in OrderManagementVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "GBP",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdatedStripePaymentID(OrderManagementVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);      
        }

        public IActionResult PaymentConfirmation(int orderHeaderid)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderid);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                //check strip status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdatedStripePaymentID(orderHeaderid, orderHeader.SessionId, session.PaymentIntentId);
                   //
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderid, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            return View(orderHeaderid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetails( )
        {

            var orderHEaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderManagementVM.OrderHeader.Id,tracked:false);
            orderHEaderFromDb.Name = OrderManagementVM.OrderHeader.Name;
            orderHEaderFromDb.PhoneNumber = OrderManagementVM.OrderHeader.PhoneNumber;  
            orderHEaderFromDb.StreetAddress = OrderManagementVM.OrderHeader.StreetAddress;  
            orderHEaderFromDb.City = OrderManagementVM.OrderHeader.City;    
            orderHEaderFromDb.Borough = OrderManagementVM.OrderHeader.Borough;  
            orderHEaderFromDb.PostCode = OrderManagementVM.OrderHeader.PostCode;

            if (OrderManagementVM.OrderHeader.Carrier != null)
            {
                orderHEaderFromDb.Carrier = OrderManagementVM.OrderHeader.Carrier;
            }
            if(OrderManagementVM.OrderHeader.TrackingNumber != null)
            {
                orderHEaderFromDb.TrackingNumber = OrderManagementVM.OrderHeader.TrackingNumber;    
            }
            _unitOfWork.OrderHeader.Update(orderHEaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction("Details", "OrderManagement", new { orderId = orderHEaderFromDb.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProccessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderManagementVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction("Details", "OrderManagement", new { orderId = OrderManagementVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderManagementVM.OrderHeader.Id, tracked: false);
            orderHeader.TrackingNumber = OrderManagementVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderManagementVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction("Details", "OrderManagement", new { orderId = OrderManagementVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderManagementVM.OrderHeader.Id, tracked: false);
            if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId,
                };
                 var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);

            }
            _unitOfWork.Save();
         

            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", "OrderManagement", new { orderId = OrderManagementVM.OrderHeader.Id });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");

            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}