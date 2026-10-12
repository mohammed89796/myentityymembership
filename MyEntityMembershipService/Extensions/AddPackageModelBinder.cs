using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

public class AddPackageModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var request = bindingContext.HttpContext.Request;

        // Extract file, allow it to be null if not present
        var file = request.Form.Files.GetFile("imageUrl");

        // Extract JSON data
        var packageDtoJson = request.Form["packageDtoJson"].FirstOrDefault();

        if (string.IsNullOrEmpty(packageDtoJson))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var packageDto = JsonSerializer.Deserialize<AddPackageDto>(packageDtoJson, options);

        // Set the result with file as nullable
        bindingContext.Result = ModelBindingResult.Success(new
        {
            PackageDto = packageDto,
            File = file // This will be null if the file is not uploaded
        });

        return Task.CompletedTask;
    }
}
