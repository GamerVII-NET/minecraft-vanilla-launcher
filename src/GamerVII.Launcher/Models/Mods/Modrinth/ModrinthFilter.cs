using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;

namespace GamerVII.Launcher.Models.Mods.Modrinth;

public class ModrinthFilter<T> : IFilter<T> where T : IFilterItem
{
    private readonly ICollection<T> _filterParameters = new List<T>();
    private string _query = string.Empty;

    public int Count => _filterParameters.Count;

    int ICollection<T>.Count => _filterParameters.Count;

    bool ICollection<T>.IsReadOnly => false;

    public string? Query {
        get => _query;
        set => _query = value;
    }
    public int Limit { get; set; } = 20;
    public int Offset { get; set; }

    public void Add(T item)
    {
        _filterParameters.Add(item);
    }
    public void AddIfNotExists(T item)
    {
        if (_filterParameters.FirstOrDefault(c => c.Key == item.Key && c.Value == item.Value) == null)
        {
            _filterParameters.Add(item);
        }
    }

    public void Clear()
    {
        _filterParameters.Clear();
    }

    public bool Contains(T item)
    {
        return _filterParameters.Contains(item) ||
               _filterParameters.FirstOrDefault(c => c.Key == item.Key && c.Value == item.Value) != null;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _filterParameters.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return _filterParameters.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _filterParameters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _filterParameters.GetEnumerator();
    }

    public string GetParametersString(string endpointName)
    {
        if (string.IsNullOrWhiteSpace(endpointName))
        {
            throw new ArgumentException(nameof(endpointName));
        }

        var stringBuilder = new StringBuilder();

        #region Private methods

        void CheckRequiredParam()
        {
            if (stringBuilder is { Length: 0 })
            {
                stringBuilder.Append($"{endpointName}?");
            }
            else
            {
                stringBuilder.Append('&');
            }
        }

        #endregion

        if (!string.IsNullOrWhiteSpace(_query))
        {
            CheckRequiredParam();
            stringBuilder.Append($"query={_query}");
        }

        if (_filterParameters.Count > 0)
        {
            // facets=[["project_type:mod"],["categories:forge"],["categories:decoration"]]
            CheckRequiredParam();

            stringBuilder.Append("facets=[");

            stringBuilder.AppendJoin(',', _filterParameters);

            stringBuilder.Append(']');
        }

        if (Limit > 0)
        {
            CheckRequiredParam();
            stringBuilder.Append($"limit={Limit}");
        }

        if (Offset > 0)
        {
            CheckRequiredParam();
            stringBuilder.Append($"offset={Offset}");
        }

        return stringBuilder.ToString();
    }
}
