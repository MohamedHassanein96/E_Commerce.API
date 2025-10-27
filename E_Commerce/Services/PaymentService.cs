namespace E_Commerce.Services
{
    public class PaymentService(UserManager<ApplicationUser> _userManager, ApplicationDbContext _context) :IPaymentService
    {
        public async Task<OneOf<PayResponse, ErrorResponse>> CreateCheckoutSessionAsync(string userId,CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return new ErrorResponse("Invalid user.", StatusCodes.Status404NotFound);


            var cartItems = await _context.Carts.Where(x => x.ApplicationUserId == userId).Include(x => x.Product).ToListAsync(cancellationToken);

            if (!cartItems.Any())
                return new ErrorResponse("Cart is empty.", StatusCodes.Status404NotFound);

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var order = new Order
                {
                    ApplicationUserId = userId,
                    Items = cartItems.Select(x => new OrderItem
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        UnitPrice = x.Product.Price
                    }).ToList(),
                    PaymentStatus = PaymentStatus.Pending
                };

                foreach (var item in cartItems)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);

                    if (product is null)
                        throw new Exception($"Product {item.Product.Name} not found.");


                    var userReserved = item.Quantity;


                    var reservedByOthers = product.ReservedStock - userReserved;
                    if (reservedByOthers < 0) 
                        reservedByOthers = 0;


                    var availableForCurrentUser = product.AvailableStock - reservedByOthers;

                    if (availableForCurrentUser < item.Quantity)
                        throw new InsufficientStockException(item.Product.Name, item.Quantity, availableForCurrentUser);



                    if (product.Version != item.Product.Version)
                        throw new Exception($"Product {item.Product.Name} has been modified.");


                    product.Version++;
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);


                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = "https://localhost:4200/checkout/success?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = $"{"https://localhost:4200"}/checkout/cancel",
                };

                foreach (var item in cartItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            },
                            UnitAmount = (long)item.Product.Price * 100,
                        },
                        Quantity = item.Quantity,
                    });
                }
                var service = new SessionService();
                var session = await service.CreateAsync(options);

                order.StripeSessionId = session.Id;
                await _context.SaveChangesAsync(cancellationToken);

                // حذف محتويات الكارت بعد تسجيل الأوردر
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                var invoiceUrl = $"https://yourapp.com/invoices/{session.Id}.pdf";

                return new PayResponse("Success", session.Id, null, session.Url);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

    }
}
