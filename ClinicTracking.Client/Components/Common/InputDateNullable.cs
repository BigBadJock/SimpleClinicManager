using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace ClinicTracking.Client.Components.Common;

/// <summary>
/// Custom InputDate component that properly handles nullable DateTime values
/// This resolves validation issues when empty date fields are submitted
/// </summary>
public class InputDateNullable : InputBase<DateTime?>
{
    protected override bool TryParseValueFromString(string? value, out DateTime? result, out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = null;
            validationErrorMessage = string.Empty;
            return true;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
        {
            result = parsedDateTime;
            validationErrorMessage = string.Empty;
            return true;
        }

        result = null;
        validationErrorMessage = $"The value '{value}' is not a valid date.";
        return false;
    }

    protected override string? FormatValueAsString(DateTime? value)
    {
        return value?.ToString("yyyy-MM-dd");
    }

    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "date");
        builder.AddAttribute(3, "class", CssClass);
        builder.AddAttribute(4, "value", FormatValueAsString(CurrentValue));
        builder.AddAttribute(5, "onchange", Microsoft.AspNetCore.Components.EventCallback.Factory.CreateBinder<string?>(
            this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        builder.CloseElement();
    }
}