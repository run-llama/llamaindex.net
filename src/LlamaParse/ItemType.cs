using System;

namespace LlamaParse;

[Flags]
public enum ItemType
{
    /// <summary>
    /// No item type.
    /// </summary>
    None = 0,

    /// <summary>
    /// Image item type.
    /// </summary>
    Image = 1,

    /// <summary>
    /// Table item type.
    /// </summary>
    Table = 2,
}
