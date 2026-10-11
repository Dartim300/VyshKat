using VyshKat.App.Ui;
using VyshKat.Core.Assets;

namespace VyshKat.App.Catalog;

public sealed record AssetKind(
    string Title,
    string InventoryPrefix,
    AssetGroup Group,
    Func<string, ConsolePrompter, Asset> Create);
