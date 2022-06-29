using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebStore.Domain.Entities.Base;
using WebStore.Domain.Entities.Base.Interfaces;

namespace WebStore.Domain.Entities;

/// <summary>Продукт</summary>
[Index(nameof(Name), IsUnique = false)]
public class Product : NamedEntity, IOrderedEntity
{
    /// <summary>Порядок</summary>
    public int Order { get; set; }

    /// <summary>Идентификатор секции</summary>
    public int SectionId { get; set; }

    /// <summary>Секция</summary>
    [Required]
    [ForeignKey(nameof(SectionId))]
    public Section Section { get; set; } = null!;

    /// <summary>Идентификатор бренда</summary>
    public int? BrandId { get; set; }

    /// <summary>Бренд</summary>
    [ForeignKey(nameof(BrandId))]
    public Brand? Brand { get; set; }

    /// <summary>Ссылка на фотографию</summary>
    [Required]
    public string ImageUrl { get; set; } = null!;

    /// <summary>Цена</summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}
