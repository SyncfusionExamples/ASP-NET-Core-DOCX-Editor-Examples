# Syncfusion&reg; ASP.NET Core DOCX Editor – Side by Side Comparison

## Introduction

This repository contains an ASP.NET Core application that demonstrates how to compare two Word documents using the Syncfusion<sup style="font-size:70%">&reg;</sup> [ASP.NET Core DOCX Editor](https://www.syncfusion.com/docx-editor-sdk/asp-net-core-docx-editor?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples) (Document Editor). The sample allows users to upload an original and a revised document, compare them using the Syncfusion .NET Word Library (DocIO), and view the comparison results with revision tracking side by side.

### Features

- Upload an original Word document (DOCX or SFDT).
- Upload a revised Word document (DOCX or SFDT).
- Compare both documents using the Syncfusion .NET Word Library (DocIO).
- View tracked revisions in the comparison result.
- Download the compared document with tracked changes preserved.
- Switch between:
  - Original vs Compared document
  - Original vs Revised document

---

# How to Run the Application

## Prerequisites

Before running the sample, ensure the following software is installed:

- .NET SDK (10.0 or later)

## Configuration

No additional configuration is required.

The sample uses the server-side web API (`api/documenteditor/Import` and `api/documenteditor/CompareDocuments`) implemented in the `Controllers/DocumentEditorController.cs` file to import and compare documents using the Syncfusion .NET Word Library (DocIO).

## Run the Application

Start the ASP.NET Core application.

```bash
dotnet run
```

The application will launch in your default browser.

If it does not open automatically, browse to the URL shown in the terminal (for example, `https://localhost:<port>`).

---

## Verify the Sample

1. Launch the application.
2. Click **Choose file** under **Original document** and select a DOCX (or SFDT) file.
3. Click **Choose file** under **Revised document** and select another version of the same document.
4. Click **Compare**.
5. The application sends both documents to the server-side web API, which compares them using DocIO.
6. The comparison result is displayed in the right-side editor with tracked revisions.
7. Toggle between:
   - **Original vs Compared** (Show comparison results with tracked changes)
   - **Original vs Revised**
8. Review insertions, deletions, and formatting changes using the **Show/Hide Review Pane** option.
9. Click **Download** to save the compared document with tracked changes preserved.

---
## Demo

<p align="center">
  <img
    src="images/compare-documents-demo-react-docx-editor.gif"
    alt="ASP.NET Core DOCX Editor Compare Documents Demo"
    width="1000" />
</p>

---
# Project Workflow

The application follows the workflow below:

1. User uploads the original document.
2. User uploads the revised document.
3. Both documents are sent to the server-side web API.
4. The service:
   - Imports the DOCX files.
   - Compares both documents using DocIO.
   - Generates a document with tracked revisions.
5. The comparison result is returned in SFDT format.
6. The ASP.NET Core application loads the original document in the left editor.
7. The compared document (or revised document) is loaded in the right editor for side-by-side review.

```
Original DOCX
        │
        │
        ▼
ASP.NET Core Application
        │
        │
        ▼
Server-side Web API (Syncfusion .NET Word Library - DocIO)
        │
        ├── Import Original
        ├── Import Revised
        └── Compare Documents
                │
                ▼
      Compared SFDT Document
                │
                ▼
ASP.NET Core DOCX Editor
        │
        ├── Left Editor  → Original
        └── Right Editor → Compared / Revised
```

---

# Resources

- **Product page:**   [Syncfusion® ASP.NET Core DOCX Editor](https://www.syncfusion.com/docx-editor-sdk/asp-net-core-docx-editor?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples) 

- **Documentation:**   [Syncfusion® ASP.NET Core DOCX Editor - Documentation](https://help.syncfusion.com/document-processing/word/word-processor/asp-net-core/overview?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples) 

- **Online demo:**   [Syncfusion® ASP.NET Core DOCX Editor - Online demo](https://document.syncfusion.com/demos/docx-editor/asp-net-core/documenteditor/default?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples#/tailwind3)

# Support and feedback 

For any other queries, reach our [Syncfusion® support team](https://support.syncfusion.com/?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples) or post the queries through the [community forums](https://www.syncfusion.com/forums?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples). 

Request new feature through [Syncfusion® feedback portal](https://www.syncfusion.com/feedback?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples). 

# License

This is a commercial product and requires a paid license for possession or use Syncfusion's licensed software, including this component, is subject to the terms and conditions of [Syncfusion's EULA](https://www.syncfusion.com/license/studio/syncfusion_essential_studio_eula.pdf?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples). You can purchase a licnense [here](https://www.syncfusion.com/sales/products?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples) or start a free 30-day trial [here](https://www.syncfusion.com/account/manage-trials/start-trials?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-core-docx-editor-examples).
