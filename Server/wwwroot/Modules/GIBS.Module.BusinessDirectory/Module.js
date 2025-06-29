/* Module Script */
var GIBS = GIBS || {};

GIBS.BusinessDirectory = {
};

window.interopFormValid = function (form) {
    if (form && form instanceof HTMLFormElement) {
        return form.checkValidity();
    }
    return false;
};

window.uploadFiles = function (posturl, folder, name) {
    var files = document.getElementById(name + 'FileInput').files;
    var progressinfo = document.getElementById(name + 'ProgressInfo');
    var progressbar = document.getElementById(name + 'ProgressBar');
    var filename = '';

    for (var i = 0; i < files.length; i++) {
        var FileChunk = [];
        var file = files[i];
        var MaxFileSizeMB = 1;
        var BufferChunkSize = MaxFileSizeMB * (1024 * 1024);
        var FileStreamPos = 0;
        var EndPos = BufferChunkSize;
        var Size = file.size;

        progressbar.setAttribute("style", "visibility: visible;");

        if (files.length > 1) {
            filename = file.name;
        }

        while (FileStreamPos < Size) {
            FileChunk.push(file.slice(FileStreamPos, EndPos));
            FileStreamPos = EndPos;
            EndPos = FileStreamPos + BufferChunkSize;
        }

        var TotalParts = FileChunk.length;
        var PartCount = 0;

        while (FileChunk.length > 0) {
            var Chunk = FileChunk.shift();
            PartCount++;
            var FileName = file.name + ".part_" + PartCount + "_" + TotalParts;

            var data = new FormData();
            data.append('folder', folder);
            data.append('file', Chunk, FileName);
            var request = new XMLHttpRequest();
            request.open('POST', posturl, true);
            request.upload.onloadstart = function (e) {
                progressbar.value = 0;
                progressinfo.innerHTML = filename + ' 0% ';
            };
            request.upload.onprogress = function (e) {
                var percent = Math.ceil((e.loaded / e.total) * 100);
                progressbar.value = (percent / 100);
                progressinfo.innerHTML = filename + '[ ' + PartCount + '] ' + percent + '% ';
            };
            request.upload.onloadend = function (e) {
                progressbar.value = 1;
                progressinfo.innerHTML = filename + ' 100% ';
            };
            request.send(data);
        }
    }
};