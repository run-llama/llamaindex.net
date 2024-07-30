using System;

namespace LlamaParse;

/// <summary>
/// Represents the type of item in a document.
/// </summary>
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
