using System.Collections.Generic;

namespace GamerVII.Launcher.Models.Mods;

public interface IFilter<T>: ICollection<T>
{
    public int Limit { get; set; }

    public int Offset { get; set; }

    public string? Query { get; set; }

    public string GetParametersString(string endPoint);


    public void AddIfNotExists(T item);

}
