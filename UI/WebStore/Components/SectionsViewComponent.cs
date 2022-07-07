using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components;

public class SectionsViewComponent : ViewComponent
{
    private readonly IProductData _ProductData;

    public SectionsViewComponent(IProductData ProductData) => _ProductData = ProductData;

    public IViewComponentResult Invoke(string sectionId)
    {
        var id = int.TryParse(sectionId, out var _id) ? _id : (int?)null;

        return View(new SelectableSectionsViewModel
        {
            Sections = GetSections(id, out var parentSectionId),
            SectionId = id,
            ParentSectionId = parentSectionId
        });
    }

    private IEnumerable<SectionViewModel> GetSections(int? sectionId, out int? parentSectionId)
    {
        parentSectionId = null;

        var sections = _ProductData.GetSections();
        var parentSections = sections.Where(s => s.ParentId is null).OrderBy(s => s.Order);
        var parentSectionsViews = parentSections
           .Select(s => new SectionViewModel
           {
               Id = s.Id,
               Name = s.Name,
           })
           .ToArray();

        foreach (var parentSection in parentSectionsViews)
        {
            var childs = sections.Where(s => s.ParentId == parentSection.Id);
            foreach (var childSection in childs.OrderBy(s => s.Order))
            {
                if (childSection.Id == sectionId)
                {
                    parentSectionId = parentSection.Id;
                }
                    
                parentSection.ChildSections.Add(new()
                {
                    Id = childSection.Id,
                    Name = childSection.Name,
                });
            }
        }

        return parentSectionsViews;
    }
}
