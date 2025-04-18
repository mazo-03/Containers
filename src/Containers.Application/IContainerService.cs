using Container = Containers.Models.Container;

namespace Containers.Application;

public interface IContainerService
{
    IEnumerable<Container> GetAllContainers();
    bool Create(Container container);
}