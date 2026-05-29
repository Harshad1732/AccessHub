namespace AccessHub.Application.DTOs;

public record InvoiceDto(Guid Id, string Number, string CustomerName, decimal Amount, DateTime CreatedAtUtc);

public record CreateInvoiceRequest(string Number, string CustomerName, decimal Amount);
