using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebStore.TagHelpers;

[HtmlTargetElement(Attributes = ATTRIBUTE_NAME)]
public class ActiveRoute: TagHelper
{
    private const string ATTRIBUTE_NAME = "ws-active-route";
    private const string IGNORE_ACTION = "ws-ignore-action";

    [HtmlAttributeName("asp-controller")]
    public string? Controller { get; set; }

    [HtmlAttributeName("asp-action")]
    public string? Action { get; set; }

    [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
    public Dictionary<string, string> RouteValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    [ViewContext, HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.RemoveAll(ATTRIBUTE_NAME);
        var ingnoreAction = output.Attributes.RemoveAll(IGNORE_ACTION);

        if (IsActive(ingnoreAction))
        {
            MakeActive(output);
        }
    }

    private bool IsActive(bool ignoreAction)
    {
        var routeValues = ViewContext.RouteData.Values;

        var routeController = routeValues["controller"]?.ToString();
        var routeAction = routeValues["action"]?.ToString();

        var skipAction = ignoreAction && IsSelected(Action, routeAction);

        if (IsSelected(Controller, routeController) || skipAction)
        {
            return false;
        }
            
        foreach (var (key, value) in RouteValues)
        {
            if (!routeValues.ContainsKey(key) || routeValues[key]?.ToString() != value.ToString())
            {
                return false;
            }
        }
   
        return true;
    }

    private bool IsSelected(string? prev, string? actual) => prev?.Length > 0 && !string.Equals(prev, actual);

    private void MakeActive(TagHelperOutput output)
    {
        var classAttribute = output.Attributes.FirstOrDefault(attr => attr.Name == "class");

        if (classAttribute is not null)
        {
            var isActiveElement = classAttribute.Value?.ToString()?.Contains("active") == true;
            var className = isActiveElement ? "active" : null;

            output.Attributes.SetAttribute("class", $"{classAttribute.Value} {className}");
        } 
        else
        {
            output.Attributes.Add("class", "active");
        }
    }
}
