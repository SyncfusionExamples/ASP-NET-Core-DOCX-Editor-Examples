using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.EJ2.DocumentEditor;
using WDocument = Syncfusion.DocIO.DLS.WordDocument;
using WFormatType = Syncfusion.DocIO.FormatType;

namespace DocumentEditor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentEditorController : Controller
    {
        /// <summary>
        /// Imports a DOCX file and returns its SFDT (Syncfusion Document Text) JSON content.
        /// </summary>
        [Route("Import")]
        public string Import(IFormCollection data)
        {
            if (data.Files.Count == 0)
            {
                return string.Empty;
            }

            IFormFile file = data.Files[0];
            int index = file.FileName.LastIndexOf('.');
            string extension = index > -1 && index < file.FileName.Length - 1 ?
                file.FileName.Substring(index) : ".docx";

            if (extension == ".sfdt")
            {
                using (var streamReader = new StreamReader(file.OpenReadStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }

            Stream stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;

            // Convert the DOCX stream to SFDT DOM.
            WordDocument document = WordDocument.Load(stream, FormatType.Docx);
            string json = JsonConvert.SerializeObject(document);
            document.Dispose();
            return json;
        }

        /// <summary>
        /// Compares an original and a revised DOCX document and returns the compared document as SFDT content with tracked changes.
        /// </summary>
        [Route("CompareDocuments")]
        public string CompareDocuments(IFormCollection data)
        {
            if (data.Files.Count < 2) return null;
            IFormFile originalDoc = data.Files[0];
            IFormFile revisedDoc = data.Files[1];

            string type1 = Path.GetExtension(originalDoc.FileName)?.ToLower() ?? ".docx";

            string type2 = Path.GetExtension(revisedDoc.FileName)?.ToLower() ?? ".docx";

            Stream originalStream = GetDocumentStream(originalDoc, type1);

            Stream revisedStream = GetDocumentStream(revisedDoc, type2);


            using (originalStream)
            using (WDocument originalDocument = new WDocument(originalStream, WFormatType.Docx))
            using (revisedStream)
            using (WDocument revisedDocument = new WDocument(revisedStream, WFormatType.Docx))
            {

                originalDocument.Compare(revisedDocument);

                var wordDoc = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(originalDocument);

                return Newtonsoft.Json.JsonConvert.SerializeObject(wordDoc);

            }
        }

        private Stream GetDocumentStream(IFormFile file, string fileType)
        {
            if (fileType == ".docx")
            {
                return file.OpenReadStream();
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string sfdtContent = reader.ReadToEnd();
                // Convert the SFDT content to a DOCX stream for comparison.
                Stream stream = Syncfusion.EJ2.DocumentEditor.WordDocument.Save(sfdtContent, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
                stream.Position = 0;
                return stream;
            }
        }
    }
}
