using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
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
            if (data.Files.Count < 2)
            {
                return string.Empty;
            }

            IFormFile? originalFile = data.Files.FirstOrDefault(f => f.Name == "originalFile");
            IFormFile? revisedFile = data.Files.FirstOrDefault(f => f.Name == "revisedFile");
            if (originalFile == null || revisedFile == null)
            {
                return string.Empty;
            }

            string author = data.TryGetValue("author", out var authorValue) ? authorValue.ToString() : "Author";
            string dateTime = data.TryGetValue("dateTime", out var dateValue) ? dateValue.ToString() : DateTime.UtcNow.ToString("o");

            var originalStream = new MemoryStream();
            originalFile.CopyTo(originalStream);
            originalStream.Position = 0;

            var revisedStream = new MemoryStream();
            revisedFile.CopyTo(revisedStream);
            revisedStream.Position = 0;

            // Load the original and revised documents using the Syncfusion .NET Word (DocIO) library.
            WDocument originalDocument = new WDocument();
            originalDocument.Settings.SkipIncrementalSaveValidation = true;
            originalDocument.Open(originalStream, WFormatType.Automatic);

            WDocument revisedDocument = new WDocument();
            revisedDocument.Settings.SkipIncrementalSaveValidation = true;
            revisedDocument.Open(revisedStream, WFormatType.Automatic);

            // Compare the revised document against the original document.
            originalDocument.Compare(revisedDocument, author, Convert.ToDateTime(dateTime));

            // Convert the compared Word document to SFDT content.
            Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(originalDocument);
            string json = JsonConvert.SerializeObject(document);
            document.Dispose();
            originalDocument.Dispose();
            revisedDocument.Dispose();
            return json;
        }
    }
}
