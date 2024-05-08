using System.Text.Json;
using FluentAssertions;
using llamaindex.net.core.Schema;

namespace llamaindex.net.core.tests;

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
                           },
                           {
                               "score": 1.0,
                               "node": {
                                   "_node_content": "{\"id_\": \"4b8fbce2-732d-45d7-9547-b7f23254cd04\", \"embedding\": null, \"metadata\": {}, \"excluded_embed_metadata_keys\": [], \"excluded_llm_metadata_keys\": [], \"relationships\": {\"1\": {\"node_id\": \"123\", \"node_type\": \"4\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"5\": [{\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}], \"4\": {\"node_id\": \"123\", \"node_type\": \"2\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"3\": {\"node_id\": \"123\", \"node_type\": \"3\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"2\": {\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}}, \"text\": \"('Netherlands', 4)\", \"start_char_idx\": null, \"end_char_idx\": null, \"text_template\": \"{metadata_str}\\n\\n{content}\", \"metadata_template\": \"{key}: {value}\", \"metadata_seperator\": \"\\n\", \"class_name\": \"TextNode\"}",
                                   "_node_type": "TextNode",
                                   "document_id": "123",
                                   "doc_id": "123",
                                   "ref_doc_id": "123"
                               }
                           },
                           {
                               "score": 1.0,
                               "node": {
                                   "_node_content": "{\"id_\": \"d769cc10-2fa6-4f1e-ac6a-e6f83065c261\", \"embedding\": null, \"metadata\": {}, \"excluded_embed_metadata_keys\": [], \"excluded_llm_metadata_keys\": [], \"relationships\": {\"1\": {\"node_id\": \"123\", \"node_type\": \"4\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"5\": [{\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}], \"4\": {\"node_id\": \"123\", \"node_type\": \"2\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"3\": {\"node_id\": \"123\", \"node_type\": \"3\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"2\": {\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}}, \"text\": \"('Belarus', 4)\", \"start_char_idx\": null, \"end_char_idx\": null, \"text_template\": \"{metadata_str}\\n\\n{content}\", \"metadata_template\": \"{key}: {value}\", \"metadata_seperator\": \"\\n\", \"class_name\": \"TextNode\"}",
                                   "_node_type": "TextNode",
                                   "document_id": "123",
                                   "doc_id": "123",
                                   "ref_doc_id": "123"
                               }
                           },
                           {
                               "score": 1.0,
                               "node": {
                                   "_node_content": "{\"id_\": \"e4d239da-701a-4e23-a66b-c89e666ddffe\", \"embedding\": null, \"metadata\": {}, \"excluded_embed_metadata_keys\": [], \"excluded_llm_metadata_keys\": [], \"relationships\": {\"1\": {\"node_id\": \"123\", \"node_type\": \"4\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"5\": [{\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}], \"4\": {\"node_id\": \"123\", \"node_type\": \"2\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"3\": {\"node_id\": \"123\", \"node_type\": \"3\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}, \"2\": {\"node_id\": \"123\", \"node_type\": \"1\", \"metadata\": {}, \"hash\": null, \"class_name\": \"RelatedNodeInfo\"}}, \"text\": \"('Dominican Republic', 4)\", \"start_char_idx\": null, \"end_char_idx\": null, \"text_template\": \"{metadata_str}\\n\\n{content}\", \"metadata_template\": \"{key}: {value}\", \"metadata_seperator\": \"\\n\", \"class_name\": \"TextNode\"}",
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

        nodes.Should().BeEquivalentTo(new[]
        {
            new NodeWithScore(new TextNode(
                "ff72a736-fa0e-4587-90e8-3aa22f87a826", 
                text:"('zunigavanessa@smith.info',)"
                ), 0.9),
            new NodeWithScore(new TextNode(
                "d77305c6-26b2-4737-bf1f-e6e7d15b1fc3", 
                text:""
                ), 0.9),
        });
    }
}