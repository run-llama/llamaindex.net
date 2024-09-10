using System.Text.Json;
using FluentAssertions;
using LlamaIndex.Core.Schema;

namespace LlamaIndex.Core.Tests;

public class TestNodeWithScoreTest
{

    [Fact]
    public void can_deserialize_from_json()
    {
        var json = """
                   {
                       "nodes": [
                           {
                               "score": 1.0,
                               "node": {
                                   "_node_content": "{\"id_\": \"679e7994-761b-4f49-a2fc-948f0f477c57\", \"embedding\": null, \"metadata\": {}, \"excluded_embed_metadata_keys\": [], \"excluded_llm_metadata_keys\": [], \"relationships\": {\"1\": {\"node_id\": \"123\", \"node_type\": \"4\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"5\": [{\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}], \"4\": {\"node_id\": \"123\", \"node_type\": \"2\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"3\": {\"node_id\": \"123\", \"node_type\": \"3\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"2\": {\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}}, \"text\": \"('Solomon Islands', 8)\", \"start_char_idx\": null, \"end_char_idx\": null, \"text_template\": \"{metadata_str}\\n\\n{content}\", \"metadata_template\": \"{key}: {value}\", \"metadata_seperator\": \"\\n\", \"class_name\": \"TextNode\"}",
                                   "_node_type": "TextNode",
                                   "document_id": "123",
                                   "doc_id": "123",
                                   "ref_doc_id": "123"
                               }
                           },
                           {
                               "score": 1.0,
                               "node": {
                                   "_node_content": "{\"id_\": \"88250f2c-165c-4395-b3ab-811474c8039d\", \"embedding\": null, \"metadata\": {}, \"excluded_embed_metadata_keys\": [], \"excluded_llm_metadata_keys\": [], \"relationships\": {\"1\": {\"node_id\": \"123\", \"node_type\": \"4\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"5\": [{\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}], \"4\": {\"node_id\": \"123\", \"node_type\": \"2\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"3\": {\"node_id\": \"123\", \"node_type\": \"3\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"2\": {\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}}, \"text\": \"('Togo', 4)\", \"start_char_idx\": null, \"end_char_idx\": null, \"text_template\": \"{metadata_str}\\n\\n{content}\", \"metadata_template\": \"{key}: {value}\", \"metadata_seperator\": \"\\n\", \"class_name\": \"TextNode\"}",
                                   "_node_type": "TextNode",
                                   "document_id": "123",
                                   "doc_id": "123",
                                   "ref_doc_id": "123"
                               }
                           }
                       ]
                   }
                   """;

        var root = JsonDocument.Parse(json);

        var nodes = root.RootElement.GetProperty("nodes").Deserialize<NodeWithScore[]>();

        nodes.Should().BeEquivalentTo([

            new NodeWithScore
            (
                new TextNode
                (
                    "679e7994-761b-4f49-a2fc-948f0f477c57",

                    text: "('Solomon Islands', 8)"
                )
                {
                    ChildNodes = [new RelatedNodeInfo("123", NodeType.TextNode)],
                    NextNode = new RelatedNodeInfo("123", NodeType.Index),
                    ParentNode = new RelatedNodeInfo("123", NodeType.Image),
                    PreviousNode = new RelatedNodeInfo("123", NodeType.TextNode),
                    SourceNode = new RelatedNodeInfo("123", NodeType.Document)

                },
                1.0
            ),
            new NodeWithScore
            (
                new TextNode
                (

                    id: "88250f2c-165c-4395-b3ab-811474c8039d",
                    text: "('Togo', 4)"
                )
                {
                    ChildNodes = [new RelatedNodeInfo("123", NodeType.TextNode)],
                    NextNode = new RelatedNodeInfo("123", NodeType.Index),
                    ParentNode = new RelatedNodeInfo("123", NodeType.Image),
                    PreviousNode = new RelatedNodeInfo("123", NodeType.TextNode),
                    SourceNode = new RelatedNodeInfo("123", NodeType.Document),

                },
                1.0
            )
        ]);
    }
}
