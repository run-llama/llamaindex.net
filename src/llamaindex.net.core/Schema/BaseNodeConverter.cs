using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace llamaindex.net.core.Schema;

public class BaseNodeConverter : JsonConverter<BaseNode>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(BaseNode).IsAssignableFrom(typeToConvert);
    }

    public override BaseNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;
        var nodeType = root.GetProperty("_node_type").GetString();
        var nodeContentString = root.GetProperty("_node_content").GetString();
        var nodeContent = JsonDocument.Parse(nodeContentString!);
        var documentId = GetStringPropertyValue("document_id", root);
        var refDocId = GetStringPropertyValue("ref_doc_id", root);
        var docId = GetStringPropertyValue("doc_id", root);

        var node = nodeType switch
        {
            "TextNode" => DeserializeTextNode(nodeContent, documentId, docId, refDocId),
            _ => throw new NotSupportedException($"Node type {nodeType} is not supported")
        };

        if (nodeContent.RootElement.TryGetProperty("relationships", out var relationshipProperty))
        {
            if (relationshipProperty.ValueKind != JsonValueKind.Null)
            {
                JsonElement? nodeRelationship;
                var relationships = relationshipProperty.Deserialize<Dictionary<string, JsonElement?>>();
                // source = 1
                if (relationships!.TryGetValue("1", out nodeRelationship) && nodeRelationship is not null)
                {
                    node.SourceNode = CreateRelateNodeInfo(nodeRelationship.Value);
                }

                if (relationships!.TryGetValue("2", out nodeRelationship) && nodeRelationship is not null)
                {
                    node.PreviousNode = CreateRelateNodeInfo(nodeRelationship.Value);

                }

                if (relationships!.TryGetValue("3", out nodeRelationship) && nodeRelationship is not null)
                {
                    node.NextNode = CreateRelateNodeInfo(nodeRelationship.Value);

                }

                if (relationships!.TryGetValue("4", out nodeRelationship) && nodeRelationship is not null)
                {
                    node.ParentNode = CreateRelateNodeInfo(nodeRelationship.Value);

                }

                if (relationships!.TryGetValue("5", out nodeRelationship) && nodeRelationship is not null)
                {
                    var relatedNodes = new List<RelatedNodeInfo>();
                    foreach (var nodeRelationshipElement in nodeRelationship.Value.EnumerateArray())
                    {
                        relatedNodes.Add(CreateRelateNodeInfo(nodeRelationshipElement));
                    }
                    node.ChildNodes = relatedNodes.ToArray();

                }

            }
        }


        return node;
    }

    private RelatedNodeInfo CreateRelateNodeInfo( JsonElement relationships)
    {
        var nodeId = GetStringPropertyValue("node_id", relationships);
        var nodeTypeString = GetStringPropertyValue("node_type", relationships);
        var metadata = relationships.GetProperty("metadata").Deserialize<Dictionary<string, object>>();
        if (metadata is not null && metadata.Count == 0)
        {
            metadata = null;
        }
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            throw new InvalidOperationException("Node Id is required");
        }
        if (string.IsNullOrWhiteSpace(nodeTypeString))
        {
           throw new InvalidOperationException("Node Type is required");
        }
        var  nodeType = (NodeType)Convert.ToInt32(nodeTypeString);
        return new RelatedNodeInfo(nodeId!, nodeType, metadata);
    }

    private BaseNode DeserializeTextNode(JsonDocument nodeContent, string? documentId, string? docId, string? refDocId)
    {
        var root = nodeContent.RootElement;
        var id = root.GetProperty("id_").GetString();
        var metadata = root.GetProperty("metadata").Deserialize<Dictionary<string, object>>();
        var text = root.GetProperty("text").GetString();
        int? startCharIndex = GetIntPropertyValue("start_char_idx", root);
        int? endCharIdx = GetIntPropertyValue("end_char_idx", root);



        return new TextNode(id!, text:text, endCharIdx: endCharIdx, startCharIndex: startCharIndex, metadata: metadata, documentId:documentId, docId:docId, refDocId:refDocId);
    }

    private static int? GetIntPropertyValue(string propertyName, JsonElement root)
    {
        if (root.TryGetProperty(propertyName, out var jsonProperty))
        {
            if (jsonProperty.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (jsonProperty.ValueKind == JsonValueKind.Number)
            {
                return jsonProperty.GetInt32();
            }

            if (jsonProperty.ValueKind == JsonValueKind.String)
            {
                var value = jsonProperty.GetString();
                if (value is not null)
                {
                    if (!value.Equals("none", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(jsonProperty.GetString(), out var intValue))
                        {
                            return intValue;
                        }
                    }
                }
            }
        }

        return null;
    }

    private static string? GetStringPropertyValue(string propertyName, JsonElement root)
    {
        string? propertyValue = null;
        if (root.TryGetProperty(propertyName, out var jsonProperty))
        {
            if (jsonProperty.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            var value = jsonProperty.GetString();
            if (value is not null)
            {
                if (!value.Equals("none", StringComparison.OrdinalIgnoreCase))
                {
                    propertyValue = value;
                }
            }
        }

        return propertyValue;
    }

    public override void Write(Utf8JsonWriter writer, BaseNode value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}