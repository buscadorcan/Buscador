
    window.downloadFile = (fileName, fileType, base64) => {
        const link = document.createElement("a");
    link.href = `data:${fileType};base64,` + base64;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    };
    window.downloadExcel = (fileName, base64) => {
        const link = document.createElement("a");
    link.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + base64;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    };

