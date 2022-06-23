using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities.Base;
using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities;

/// <summary>Секция</summary>
[Index(nameof(Name), IsUnique = false)]
public class Section : NamedEntity, IOrderedEntity
{
    /// <summary>Порядок</summary>
    public int Order { get; set; }

    /// <summary>Идентификатор родительской секции</summary>
    public int? ParentId { get; set; }

    /// <summary>Родительская секция</summary>
    [ForeignKey(nameof(ParentId))]
    public Section? Parent { get; set; }

    /// <summary>Продукты</summary>
    public ICollection<Product> Products { get; set; } = new HashSet<Product>();
}
