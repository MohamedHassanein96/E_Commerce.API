
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace E_Commerce.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<InvoiceService> _logger;
        private readonly ApplicationDbContext _context;

        public InvoiceService(IWebHostEnvironment env, ILogger<InvoiceService> logger,ApplicationDbContext context)
        {
            _env = env;
            _logger = logger;
            _context = context;
        }

        public async Task<string> GenerateInvoiceAsync(Order order, CancellationToken cancellationToken = default)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            try
            {
                var now = DateTime.UtcNow;
                var invoicesFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "invoices", now.Year.ToString(), now.Month.ToString("D2"));
                Directory.CreateDirectory(invoicesFolder);

                var filename = $"Invoice_Order_{order.Id}_{now:yyyyMMddHHmmss}.pdf";
                var tempPath = Path.Combine(Path.GetTempPath(), filename);
                var finalPath = Path.Combine(invoicesFolder, filename);

                byte[] pdfBytes =await CreateInvoicePdfBytesAsync(order);

                await File.WriteAllBytesAsync(tempPath, pdfBytes, cancellationToken);

                if (File.Exists(finalPath))
                    File.Delete(finalPath); 
                File.Move(tempPath, finalPath);

                var relativePath = Path.Combine("invoices", now.Year.ToString(), now.Month.ToString("D2"), filename).Replace("\\", "/");
                return relativePath;
            }
            catch (OperationCanceledException)
            {
                // لو تم إلغاء العملية
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate invoice for order {OrderId}", order.Id);
                throw;
            }
        }

        private async Task<byte[]> CreateInvoicePdfBytesAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Column(header =>
                    {
                        header.Item().Text("My Shop").FontSize(20).Bold();
                        header.Item().Text($"Invoice - Order #{order.Id}").FontSize(14).Bold();
                        header.Item().Text($"Date: {order.CreatedAt:yyyy-MM-dd HH:mm} UTC");
                        header.Item().Text($"Status: {order.PaymentStatus}");
                    });

                    page.Content().Column(content =>
                    {
                        content.Item().LineHorizontal(1);
                        content.Item().Text("Items:").SemiBold();

                        foreach (var it in order.Items)
                        {
                            content.Item().PaddingVertical(5).Row(row =>
                            {
                                row.RelativeColumn().Text(it.Product.Name).SemiBold();
                                row.ConstantColumn(60).AlignRight().Text($"{it.Quantity}x");
                                row.ConstantColumn(100).AlignRight().Text($"{(it.UnitPrice * it.Quantity):C}");
                            });
                        }

                        content.Item().LineHorizontal(1);

                        var total = order.Items.Sum(i => i.UnitPrice * i.Quantity);
                        content.Item().Row(row =>
                        {
                            row.RelativeColumn();
                            row.ConstantColumn(150).AlignRight().Text($"Total: {total:C}").FontSize(14).Bold();
                        });

                        if (!string.IsNullOrEmpty(order.ApplicationUserId))
                        {
                            content.Item().PaddingTop(10).Text($"Customer ID: {order.ApplicationUserId}");
                        }
                    });

                    page.Footer().AlignCenter().Text(x => x.Span("Thank you for your purchase"));
                });
            });

            using var stream = new MemoryStream();

 
            var generated = document.GeneratePdf();
            await stream.WriteAsync(generated, 0, generated.Length);
            return stream.ToArray();
        }
        public async Task<byte[]> GetInvoiceFileAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null || string.IsNullOrEmpty(order.InvoicePath))
                throw new FileNotFoundException("Invoice not found.");

            var pdfPath = Path.Combine(_env.WebRootPath ?? "wwwroot", order.InvoicePath);

            if (!File.Exists(pdfPath))
                throw new FileNotFoundException("Invoice not found.", pdfPath);

            return await File.ReadAllBytesAsync(pdfPath);
        }
    }
}
