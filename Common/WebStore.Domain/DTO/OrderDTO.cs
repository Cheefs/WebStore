using System;
using System.Diagnostics.CodeAnalysis;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;

namespace WebStore.Domain.DTO;

public class OrderDTO
{
    public int Id { get; init; }
    public string Phone { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string Description { get; init; } = null!;
    public DateTimeOffset Date { get; init; }
    public IEnumerable<OrderItemDTO> Items { get; init; } = null!;
}

public class OrderItemDTO
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}

public class CreateOrderDTO
{
    public OrderViewModel Order { get; init; } = null!;
    public IEnumerable<OrderItemDTO> Items { get; init; } = null!;
}


public static class OrderDtoMapper
{
    [return: NotNullIfNotNull("item")]
    public static OrderItemDTO? ToDTO(this OrderItem item) => item is null
        ? null
        : new OrderItemDTO
        {
            Id = item.Id,
            Price = item.Price,
            Quantity = item.Quantity,
            ProductId = item.Product.Id
        };
    [return: NotNullIfNotNull("item")]
    public static OrderItem ? FromDTO(this OrderItemDTO item) => item is null
     ? null
     : new OrderItem
     {
         Id = item.Id,
         Price = item.Price,
         Quantity = item.Quantity,
         Product = new Product { Id = item.Id }
     };

    [return: NotNullIfNotNull("order")]
    public static OrderDTO? ToDTO(this Order order) => order is null
    ? null
    : new OrderDTO
    {
        Id = order.Id,
        Address = order.Address,
        Phone = order.Phone,
        Date = order.Date,
        Description = order.Description!,
        Items = order.Items.Select(ToDTO)!
    };
    [return: NotNullIfNotNull("order")]
    public static Order? FromDTO(this OrderDTO order) => order is null
     ? null
     : new Order
     {
         Id = order.Id,
         Address = order.Address,
         Phone = order.Phone,
         Date = order.Date,
         Description = order.Description!,
         Items = order.Items.Select(FromDTO).ToList()!
     };

    public static IEnumerable<OrderDTO?> ToDTO(this IEnumerable<Order?> orders) => orders.Select(ToDTO!);
    public static IEnumerable<Order?> FromDTO(this IEnumerable<OrderDTO?> orders) => orders.Select(FromDTO!);

    public static IEnumerable<OrderItemDTO> ToDTO(this CartViewModel cart) => cart.Items.Select(e => new OrderItemDTO
    {
        ProductId = e.Product.Id,
        Price = e.Product.Price,
        Quantity = e.Quantity,
    });

    public static CartViewModel ToCartView(this IEnumerable<OrderItemDTO> items) => new()
    {
        Items = items.Select(e => (new ProductViewModel { Id = e.ProductId }, e.Quantity))
    };
}
