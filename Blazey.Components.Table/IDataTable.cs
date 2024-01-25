
namespace Blazey.Components.Table;

public interface IDataTable
{
    Guid GetNext(Guid Current);
    Guid GetPrevious(Guid Current);
}