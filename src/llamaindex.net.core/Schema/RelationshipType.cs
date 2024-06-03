using System;

namespace llamaindex.core.Schema;

public enum RelationshipType
{
    Source = 1,
    Previous = 2,
    Next = 3,
    Parent = 4,
    Child = 5
}

public static class RelationshipTypeExtensions
{
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