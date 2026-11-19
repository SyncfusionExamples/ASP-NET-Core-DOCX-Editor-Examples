using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.DocumentEditor;
using WDocument = Syncfusion.DocIO.DLS.WordDocument;
using WFormatType = Syncfusion.DocIO.FormatType;

namespace CompareDocuments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentEditorController : ControllerBase
    {
        [AcceptVerbs("Post")]
        [HttpPost]
        [EnableCors("AllowAllOrigins")]
        [Route("CompareDocuments")]
        public string? CompareDocuments([FromForm] CompareParameter data)
        {
            if (data.OriginalFile == null || data.RevisedFile == null)
                return null;
            IFormFile originalFile = data.OriginalFile;
            IFormFile revisedFile = data.RevisedFile;
            try
            {
                using (WDocument revisedDocument = GetWordDocument(revisedFile))
                {
                    using (WDocument originalDocument = GetWordDocument(originalFile))
                    {
                        // Compares the original document with the revised document and highlights the changes as tracked changes.
                        originalDocument.Compare(revisedDocument);
                        // Hooks MetafileImageParsed event.
                        WordDocument.MetafileImageParsed += OnMetafileImageParsed;
                        // Loads the compared document into the WordDocument instance of the Document Editor server-side library.
                        WordDocument document = WordDocument.Load(originalDocument);
                        // Unhooks MetafileImageParsed event.
                        WordDocument.MetafileImageParsed -= OnMetafileImageParsed;
                        // Serializes the compared document as SFDT content.
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                        document.Dispose();
                        return json;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error comparing documents: " + ex.Message, ex);
            }
        }

        public class CompareParameter
        {
            public IFormFile? OriginalFile { get; set; }
            public IFormFile? RevisedFile { get; set; }
        }

        private static WDocument GetWordDocument(IFormFile file)
        {
            Stream stream = new MemoryStream();
            int index = file.FileName.LastIndexOf('.');
            string type = index > -1 && index < file.FileName.Length - 1 ?
                file.FileName.Substring(index) : ".docx";
            file.CopyTo(stream);
            stream.Position = 0;
            WDocument document;
            if (type == ".sfdt")
            {
                // Converts the SFDT content to DOCX format for comparison.
                using (var reader = new StreamReader(stream))
                {
                    string sfdtContent = reader.ReadToEnd();
                    document = WordDocument.Save(sfdtContent);
                    var outStream = new MemoryStream();
                    document.Save(outStream, WFormatType.Docx);
                    document.Close();
                    return new WDocument(outStream, WFormatType.Docx);
                }
            }
            else
            {
                // Loads the DOCX document for comparison.
                document = new WDocument(stream, GetWFormatType(type));
                stream.Dispose();
                return document;
            }
        }

        private static WFormatType GetWFormatType(string format)
        {
            return format.ToLower() switch
            {
                ".dotx" => WFormatType.Dotx,
                ".docm" => WFormatType.Docm,
                ".dotm" => WFormatType.Dotm,
                ".docx" => WFormatType.Docx,
                ".rtf" => WFormatType.Rtf,
                ".txt" => WFormatType.Txt,
                ".xml" => WFormatType.WordML,
                _ => WFormatType.Docx,
            };
        }

        private static void OnMetafileImageParsed(object? sender, MetafileImageParsedEventArgs args)
        {
            // Metafile (EMF/WMF) images are not rendered on the server, so the default handling is kept.
        }
    }
}
