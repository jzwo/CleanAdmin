using Microsoft.AspNetCore.Components;


namespace CleanAdmin.Web.Client;

public static class IconHelper
{
    public static RenderFragment TailwindIcon(string @class) => builder =>
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", $"{@class}");
        builder.CloseElement();
    };
}