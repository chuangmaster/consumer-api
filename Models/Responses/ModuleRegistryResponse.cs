namespace ConsumerApi.Models.Responses;

public class ModuleRegistryResponse
{
    public List<WidgetModuleResponse> Widgets { get; set; } = [];

    public List<MenuModuleResponse> Menus { get; set; } = [];
}
