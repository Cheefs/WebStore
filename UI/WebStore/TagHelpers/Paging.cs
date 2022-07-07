using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebStore.Domain.ViewModels;

namespace WebStore.TagHelpers;

public class Paging : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;
    public PageViewModel PageModel { get; set; } = null!;

    public string PageAction { get; set; } = null!;

    [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
    public Dictionary<string, object> PageUrlValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [ViewContext, HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public Paging(IUrlHelperFactory UrlHelperFactory) => _urlHelperFactory = UrlHelperFactory;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var ul = new TagBuilder("ul");
        ul.AddCssClass("pagination");

        var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
        for (var i = 1; i <= PageModel.TotalPages; i++)
        {
            ul.InnerHtml.AppendHtml(CreateElement(i, urlHelper));
        }
        
        output.Content.AppendHtml(ul);
    }

    private TagBuilder CreateElement(int PageNumber, IUrlHelper Url)
    {
        var li = new TagBuilder("li");
        var a = new TagBuilder("a");
        a.InnerHtml.AppendHtml(PageNumber.ToString());

        if (PageNumber != PageModel.Page)
        {
            PageUrlValues["PageNumber"] = PageNumber;
            a.Attributes["href"] = Url.Action(PageAction, PageUrlValues);
        } 
        else
        {
            li.AddCssClass("active");
        }

        li.InnerHtml.AppendHtml(a);
        return li;
    }
}