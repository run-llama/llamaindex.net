namespace llamaindex.net.core.tests;

public class SchemaSerializationTests
{
    [Fact]
    public void Test1()
    {
        var json = """
                   {
                       "nodes": [
                           {
                               "score": 0.9,
                               "node": {
                                   "_node_content": "{\"id_\": \"ff72a736-fa0e-4587-90e8-3aa22f87a826\", \"embedding\": null, \"metadata\": {}, \"excluded_embed_metadata_keys\": [], \"excluded_llm_metadata_keys\": [], \"relationships\": {}, \"text\": \"('zunigavanessa@smith.info',)\", \"start_char_idx\": null, \"end_char_idx\": null, \"text_template\": \"{metadata_str}\\n\\n{content}\", \"metadata_template\": \"{key}: {value}\", \"metadata_seperator\": \"\\n\", \"class_name\": \"TextNode\"}",
                                   "_node_type": "TextNode",
                                   "document_id": "None",
                                   "doc_id": "None",
                                   "ref_doc_id": "None"
                               }
                           },
                           {
                               "score": 0.9,
                               "node": {
                                   "_node_content": "{\"id_\": \"d77305c6-26b2-4737-bf1f-e6e7d15b1fc3\", \"embedding\": null, \"metadata\": {}, \"excluded_embed_metadata_keys\": [], \"excluded_llm_metadata_keys\": [], \"relationships\": {}, \"text\": \"('mariokhan@ryan-pope.org',)\", \"start_char_idx\": null, \"end_char_idx\": null, \"text_template\": \"{metadata_str}\\n\\n{content}\", \"metadata_template\": \"{key}: {value}\", \"metadata_seperator\": \"\\n\", \"class_name\": \"TextNode\"}",
                                   "_node_type": "TextNode",
                                   "document_id": "None",
                                   "doc_id": "None",
                                   "ref_doc_id": "None"
                               }
                           }
                       ]
                   }
                   """;
    }
}