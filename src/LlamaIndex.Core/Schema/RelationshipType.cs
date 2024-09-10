using System;

namespace LlamaIndex.Core.Schema;

/// <summary>
/// Represents the type of relationship between nodes.
/// </summary>
public enum RelationshipType
{
    Source = 1,
    Previous = 2,
    Next = 3,
    Parent = 4,
    Child = 5
}

/// <summary>
/// Provides extension methods for <see cref="RelationshipType"/>.
/// </summary>
public static class RelationshipTypeExtensions
{
    /// <summary>
    /// Converts a <see cref="RelationshipType"/> to a relationship name.
    /// </summary>
    /// <param name="relationshipType">The <see cref="RelationshipType"/></param>
    /// <returns>The name of the relationship type.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string ToRelationshipName(this RelationshipType relationshipType)
    {
        return relationshipType switch
        {
            RelationshipType.Source => "source",
            RelationshipType.Previous => "previous",
            RelationshipType.Next => "next",
            RelationshipType.Parent => "parent",
            RelationshipType.Child => "child",
            _ => throw new ArgumentOutOfRangeException(nameof(relationshipType), relationshipType, null)
        };
    }

    /// <summary>
    /// Converts a <see cref="RelationshipType"/> to a relationship key.
    /// </summary>
    /// <param name="relationshipType">The <see cref="RelationshipType"/></param>
    /// <returns>The key of the relationship type.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string ToRelationshipKey(this RelationshipType relationshipType)
    {
        return relationshipType switch
        {
            RelationshipType.Source => "1",
            RelationshipType.Previous => "2",
            RelationshipType.Next => "3",
            RelationshipType.Parent => "4",
            RelationshipType.Child => "5",
            _ => throw new ArgumentOutOfRangeException(nameof(relationshipType), relationshipType, null)
        };
    }
}
