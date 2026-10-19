// Side by Side Comparison sample - client-side logic
(function () {
    var serviceUrl = '/api/documenteditor/';
    var editor1, editor2;
    var originalFile = null, revisedFile = null;
    var compareButtonEJ, downloadButtonEJ, showRevisionsButtonEJ;

    function initializeEJButtons() {
        compareButtonEJ = new ej.buttons.Button({ isPrimary: true, disabled: true }, '#compareButton');
        downloadButtonEJ = new ej.buttons.Button({ iconCss: 'e-icons e-download', cssClass: 'e-outline e-primary', disabled: true }, '#downloadButton');
        showRevisionsButtonEJ = new ej.buttons.Button({ iconCss: 'e-icons e-eye' }, '#showRevisionsButton');
    }

    function updateButtonStates() {
        compareButtonEJ.disabled = !(originalFile != null && revisedFile != null);
        downloadButtonEJ.disabled = compareButtonEJ.disabled;
    }

    function setOriginalFileName() {
        var originalFileNameElement = document.getElementById('originalFileName');
        if (originalFileNameElement) {
            if (originalFile) {
                var size = Math.round(originalFile.size / 1024);
                originalFileNameElement.textContent = originalFile.name + ' (' + size + ' KB)';
            } else {
                originalFileNameElement.textContent = 'Supported formats: SFDT, DOCX';
            }
        }
    }

    function setRevisedFileName() {
        var revisedFileNameElement = document.getElementById('revisedFileName');
        if (revisedFileNameElement) {
            if (revisedFile) {
                var size = Math.round(revisedFile.size / 1024);
                revisedFileNameElement.textContent = revisedFile.name + ' (' + size + ' KB)';
            } else {
                revisedFileNameElement.textContent = 'Supported formats: SFDT, DOCX';
            }
        }
    }

    function showHideWaitingIndicator(show) {
        var waitingPopUp = document.getElementById('waiting-popup');
        var inActiveDiv = document.getElementById('popup-overlay');
        if (waitingPopUp && inActiveDiv) {
            inActiveDiv.style.display = show ? 'block' : 'none';
            waitingPopUp.style.display = show ? 'block' : 'none';
        }
    }

    function wireDocumentEditorScrollSync() {
        if (editor1 && editor1.documentEditor && editor2 && editor2.documentEditor) {
            editor1.documentEditor.viewChange = function () {
                var pos = editor1.documentEditor.selection.getScrollPosition();
                editor2.documentEditor.selection.setScrollPosition(pos);
            };
            editor2.documentEditor.viewChange = function () {
                var pos = editor2.documentEditor.selection.getScrollPosition();
                editor1.documentEditor.selection.setScrollPosition(pos);
            };
        }
    }

    function isSupportedFormatType(formatType) {
        return ['.docx', '.sfdt'].includes(formatType);
    }

    function openFileInEditor(file, editor) {
        return new Promise(function (resolve, reject) {
            var formatType = file.name.slice(file.name.lastIndexOf('.')).toLowerCase();
            if (formatType === '.sfdt') {
                var reader = new FileReader();
                reader.onload = function (e) {
                    if (editor && editor.documentEditor) {
                        editor.documentEditor.showRevisions = false;
                        editor.documentEditor.open(e.target.result);
                        resolve();
                    } else reject(new Error('Editor not ready'));
                };
                reader.onerror = reject;
                reader.readAsText(file);
            } else {
                var formData = new FormData();
                formData.append('file', file);

                var xhr = new XMLHttpRequest();
                xhr.open('POST', serviceUrl + 'Import', true);
                xhr.onload = function () {
                    if (xhr.status >= 200 && xhr.status < 300) {
                        var sfdtString = xhr.responseText;
                        if (editor && editor.documentEditor) {
                            editor.documentEditor.showRevisions = false;
                            editor.documentEditor.open(sfdtString);
                            resolve();
                        } else {
                            reject(new Error('Editor not ready'));
                        }
                    } else {
                        reject(new Error('Failed import'));
                    }
                };
                xhr.onerror = function () {
                    reject(new Error('Network error during import'));
                };
                xhr.send(formData);
            }
        });
    }

    function loadComparedDocumentAndOpen(originalFile, revisedFile) {
        return new Promise(function (resolve, reject) {
            var formData = new FormData();
            formData.append('originalFile', originalFile);
            formData.append('revisedFile', revisedFile);
            formData.append('author', 'Author');
            formData.append('dateTime', new Date().toISOString());
            var xhr = new XMLHttpRequest();
            xhr.open('POST', serviceUrl + 'CompareDocuments', true);
            xhr.onload = function () {
                if (xhr.status >= 200 && xhr.status < 300) {
                    var sfdtString = xhr.responseText;
                    if (editor2 && editor2.documentEditor) {
                        editor2.documentEditor.showRevisions = false;
                        editor2.documentEditor.open(sfdtString);
                        resolve();
                    } else {
                        reject(new Error('Editor not ready'));
                    }
                } else {
                    reject(new Error('Failed import'));
                }
            };
            xhr.onerror = function () {
                reject(new Error('Network error during import'));
            };
            xhr.send(formData);
        });
    }

    function onCompare(originalFile, revisedFile, showResult) {
        var formatTypeOriginalFile = originalFile.name.slice(originalFile.name.lastIndexOf('.')).toLowerCase();
        var formatTypeRevisedFile = revisedFile.name.slice(revisedFile.name.lastIndexOf('.')).toLowerCase();
        if (isSupportedFormatType(formatTypeOriginalFile) && isSupportedFormatType(formatTypeRevisedFile)) {
            downloadButtonEJ.disabled = true;
            showRevisionsButtonEJ.content = 'Show Review Pane';
            showHideWaitingIndicator(true);
            setTimeout(function () {
                var run = async function () {
                    try {
                        if (showResult) {
                            if (originalFile) await openFileInEditor(originalFile, editor1);
                            if (originalFile && revisedFile) {
                                await loadComparedDocumentAndOpen(originalFile, revisedFile);
                                downloadButtonEJ.disabled = false;
                            }
                        } else {
                            if (originalFile) await openFileInEditor(originalFile, editor1);
                            if (revisedFile) {
                                await openFileInEditor(revisedFile, editor2);
                                downloadButtonEJ.disabled = false;
                            }
                        }
                    } catch (error) {
                        alert('Failed to complete comparison: ' + error.message);
                    }
                    showHideWaitingIndicator(false);
                };
                run();
            }, 0);
        } else {
            alert('The source and revised file should be either in SFDT or DOCX. Please try again.');
            compareButtonEJ.disabled = true;
            downloadButtonEJ.disabled = true;
        }
    }

    function onDownload() {
        if (editor2 && editor2.documentEditor)
            editor2.documentEditor.save('Result', 'Docx');
    }

    document.addEventListener('DOMContentLoaded', function () {
        initializeEJButtons();

        var originalFileInput = document.getElementById('originalFileInput');
        var revisedFileInput = document.getElementById('revisedFileInput');
        var showTrackedChanges = document.getElementById('showResultCheckbox');
        var compareButton = document.getElementById('compareButton');
        var downloadButton = document.getElementById('downloadButton');
        var editor2Label = document.getElementById('rightEditorTitle');
        var removeOriginalFileSpan = document.getElementById('removeOriginalFile');
        var removeRevisedFileSpan = document.getElementById('removeRevisedFile');
        var showRevisionsButton = document.getElementById('showRevisionsButton');

        editor1 = document.getElementById('documentEditor1').ej2_instances[0];
        editor2 = document.getElementById('documentEditor2').ej2_instances[0];

        // Load the default original and revised documents.
        var fetch = new ej.base.Fetch('/scripts/compare-documents/data-compare-documents.json', 'GET');
        fetch.send().then();
        fetch.onSuccess = function (data) {
            if (typeof data === 'string') {
                data = JSON.parse(data);
            }
            editor1.documentEditor.showRevisions = false;
            editor1.documentEditor.open(data.originalDocument);
            editor1.documentEditor.documentName = 'Original Document';
            editor2.documentEditor.showRevisions = false;
            editor2.documentEditor.open(data.revisedDocument);
            editor2.documentEditor.documentName = 'Revised Document';
        };

        wireDocumentEditorScrollSync();

        originalFileInput.addEventListener('change', function (e) {
            originalFile = e.target.files[0] || null;
            updateButtonStates();
            setOriginalFileName();
        });

        revisedFileInput.addEventListener('change', function (e) {
            revisedFile = e.target.files[0] || null;
            updateButtonStates();
            setRevisedFileName();
        });

        removeOriginalFileSpan.addEventListener('click', function () {
            originalFile = null;
            originalFileInput.value = '';
            updateButtonStates();
            setOriginalFileName();
        });

        removeRevisedFileSpan.addEventListener('click', function () {
            revisedFile = null;
            revisedFileInput.value = '';
            updateButtonStates();
            setRevisedFileName();
        });

        showTrackedChanges.addEventListener('change', function (e) {
            editor2Label.textContent = e.target.checked ? 'Result Document (with tracked changes)' : 'Revised Document';
            showRevisionsButton.style.display = e.target.checked ? 'block' : 'none';
        });

        compareButton.addEventListener('click', function () {
            onCompare(originalFile, revisedFile, showTrackedChanges.checked);
        });

        downloadButton.addEventListener('click', onDownload);

        showRevisionsButton.addEventListener('click', function () {
            if (editor2) {
                editor2.documentEditor.showRevisions = !editor2.documentEditor.showRevisions;
                showRevisionsButtonEJ.content = editor2.documentEditor.showRevisions ? 'Hide Review Pane' : 'Show Review Pane';
            }
        });
    });
})();
