using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebStore.Domain.ViewModels;

namespace WebStore.TagHelpers;

public class Paging : TagHelper
{
    public PageViewModel PageModel { get; set; } = null!;

    public string PageAction { get; set; } = null!;

    [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
    public Dictionary<string, object> PageUrlValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [ViewContext, HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var ul = new TagBuilder("ul");
        ul.AddCssClass("pagination");

        for (var i = 1; i <= PageModel.TotalPages; i++)
        {
            ul.InnerHtml.AppendHtml(CreateElement(i));
        }
        
        output.Content.AppendHtml(ul);
    }

    private TagBuilder CreateElement(int PageNumber)
    {
        var li = new TagBuilder("li");
        var a = new TagBuilder("a");
        a.InnerHtml.AppendHtml(PageNumber.ToString());

        if (PageNumber != PageModel.Page)
        {
            a.Attributes["href"] = "#";
        } 
        else
        {
            li.AddCssClass("active");
        }

        PageUrlValues["PageNumber"] = PageNumber;

        var pageUrlValues = PageUrlValues
            .Select(v => (v.Key, Value: v.Value?.ToString()))
            .Where(v => v.Value?.Length > 0);

        foreach (var (key, value) in pageUrlValues)
        {
            a.MergeAttribute($"data-{key}", value);
        }
            
        li.InnerHtml.AppendHtml(a);
        return li;
    }
}