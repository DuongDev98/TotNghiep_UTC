//dialog
function ShowYesNo(title, message, callBack) {
    $('#modal-dialog').empty();
    $('#modal-dialog').append('<div class="modal-dialog" role="document">\n' +
        '    <div class="modal-content">\n' +
        '      <div class="modal-header">\n' +
        '        <h5 class="modal-title">' + title+'</h5>\n' +
        '        <button type="button" class="close" data-dismiss="modal" aria-label="Close">\n' +
        '          <span aria-hidden="true">&times;</span>\n' +
        '        </button>\n' +
        '      </div>\n' +
        '      <div class="modal-body">\n' +
        '         ' + message + '    ' +
        '      </div>\n' +
        '      <div class="modal-footer">\n' +
        '        <div id="btnYes" class="btn btn-primary">Chấp nhận</div>\n' +
        '        <button type="button" class="btn btn-danger" data-dismiss="modal">Hủy bỏ</button>\n' +
        '      </div>\n' +
        '    </div>\n' +
        '</div>');

    $('#modal-dialog').modal('show');
    $('#modal-dialog').find("#btnYes").on("click", function () {
        callBack();
        $('#modal-dialog').modal('hide');
    });
}
//Thong tin
function ShowInfo(message) {
    $('#modal-dialog').empty();
    $('#modal-dialog').append('<div class="modal-dialog" role="document">\n' +
        '    <div class="modal-content">\n' +
        '      <div class="modal-header">\n' +
        '        <h5 class="modal-title">Thông báo</h5>\n' +
        '        <button type="button" class="close" data-dismiss="modal" aria-label="Close">\n' +
        '          <span aria-hidden="true">&times;</span>\n' +
        '        </button>\n' +
        '      </div>\n' +
        '      <div class="modal-body">\n' +
        '         ' + message + '    ' +
        '      </div>\n' +
        '      <div class="modal-footer">\n' +
        '        <button type="button" class="btn btn-primary" data-dismiss="modal">OK</button>\n' +
        '      </div>\n' +
        '    </div>\n' +
        '</div>');

    $('#modal-dialog').modal('show');
}
//ShowForm
function ShowAddEditForm(title, html, urlSubmit) {
    $('#modal-dialog').empty();
    $('#modal-dialog').append('<div class="modal-dialog" role="document">\n' +
        '    <div class="modal-content">\n' +
        '      <div class="modal-header">\n' +
        '        <h5 class="modal-title">' + title + '</h5>\n' +
        '        <button type="button" class="close" data-dismiss="modal" aria-label="Close">\n' +
        '          <span aria-hidden="true">&times;</span>\n' +
        '        </button>\n' +
        '      </div>\n' +
        '      <div class="modal-body">\n' +
        '           <div class="alert alert-danger" role="alert" hidden="hidden"></div >'+
        '         ' + html + '    ' +
        '      </div>\n' +
        '   <div class="modal-footer">' +
        '       <button type="button" id="btnYes" class="btn btn-success">Lưu dữ liệu</button>' +
        '       <button type="button" class="btn btn-danger" data-dismiss="modal">Thoát</button>' +
        '   </div>' +
        '    </div>\n' +
        '</div>');
    $('#modal-dialog').modal('show');

    $('#modal-dialog').find("#btnYes").on("click", function () {
        if (urlSubmit.indexOf("MatHang/UploadAnh") > -1) {
            //upload ảnh mặt hàng
            $('#modal-dialog').find("form").submit();
        }
        else {
            //upload thông tin
            let formData = $('#modal-dialog').find("form").serializeArray();
            $.post(urlSubmit, formData, (success) => {
                if (success.length == 0) {
                    $('#modal-dialog').find(".alert-danger").attr("hidden", "hidden");
                    location.reload();
                } else {
                    $('#modal-dialog').find(".alert-danger").removeAttr("hidden");
                    $('#modal-dialog').find(".alert-danger").text(success);
                }
            });
        }
    });

    $('#modal-dialog').find("form").submit(function (event) {
        event.preventDefault();
        let formData = new FormData(this);
        $.ajax({
            url: urlSubmit,
            method: "post",
            data: formData,
            processData: false,
            contentType: false
        });
        location.reload();
    });
}
//ajax post data
function SendDataToServer(url, method, data, callBackSuccess) {
    $.ajax({
        url: window.location.origin + url,
        type: method,
        data: { "data": JSON.stringify(data) },
        success: function (result) {
            var msg = result["msg"] + "";
            if (msg == "success") {
                callBackSuccess();
            } else {
                ShowDialogInfo("Có lỗi trong quá trình xử lý, vui lòng liên hệ nhà cung cấp!\r\n" + msg);
            }
        }, error: function (error) {
            ShowDialogInfo("Có lỗi trong quá trình xử lý, vui lòng liên hệ nhà cung cấp!\r\n" + JSON.stringify(error));
        }
    });
};

//Nhóm hàng
function showNhomHang(id) {
    $.get("/NhomMatHang/Item/" + id, function (data) {
        ShowAddEditForm(id.length == 0 ? "Thêm mới nhóm hàng" : "Chỉnh sửa nhóm hàng", data, "/NhomMatHang/AddOrEdit/" + id);
    });
}

function deleteNhomHang(id) {
    ShowYesNo("Lựa chọn", "Bạn muốn xóa nhóm mặt hàng đang chọn?", () => {
        $.post("/NhomMatHang/Delete/" + id, function (data) {
            if (data.length == 0) location.reload();
            else alert(data);
        });
    });
}

//Nhà cung cấp
function showNhaCungCap(id) {
    $.get("/NhaCungCap/Item/" + id, function (data) {
        ShowAddEditForm(id.length == 0 ? "Thêm mới nhà cung cấp" : "Chỉnh sửa nhà cung cấp", data, "/NhaCungCap/AddOrEdit/" + id);
    });
}

function deleteNhaCungCap(id) {
    ShowYesNo("Lựa chọn", "Bạn muốn xóa nhóm mặt hàng đang chọn?", () => {
        $.post("/NhaCungCap/Delete/" + id, function (data) {
            if (data.length == 0) location.reload();
            else alert(data);
        });
    });
}

//Mặt hàng
function showMatHang(id) {
    $.get("/MatHang/Item/" + id, function (data) {
        ShowAddEditForm(id.length == 0 ? "Thêm mới mặt hàng" : "Chỉnh sửa mặt hàng", data, "/MatHang/AddOrEdit/" + id);
    });
}

function QuanLyAnhMatHang(id, name) {
    let html = '<form id="data" method="post" enctype="multipart/form-data"><div class="input-images"></div></form>';
    ShowAddEditForm(name, html, "/MatHang/UploadAnh/" + id);
    $.get("/MatHang/GetImage/" + id, function (dataStr) {
        let preloaded = [];
        if (dataStr != "[]") {
            let dataArr = JSON.parse(dataStr);
            for (let i = 0; i < dataArr.length; i++) {
                let item = dataArr[i];
                preloaded.push({ id: item.ID, src: item.LINK });
            }
        }
        $('.input-images').imageUploader({
            preloaded: preloaded
        });
    });
}

function deleteMatHang(id) {
    ShowYesNo("Lựa chọn", "Bạn muốn xóa mặt hàng đang chọn?", () => {
        $.post("/MatHang/Delete/" + id, function (data) {
            if (data.length == 0) location.reload();
            else alert(data);
        });
    });
}

function deleteNhapKho(id) {
    ShowYesNo("Lựa chọn", "Bạn muốn xóa phiếu nhập đang chọn?", () => {
        $.post("/NhapKho/Delete/" + id, function (data) {
            if (data.length == 0) location.reload();
            else alert(data);
        });
    });
}

function LocMatHang(nhomId, search, itemCart) {
    $.get("/MatHang/Table?page=1&nhomId=" + nhomId + "&s=" + search + "&itemCart=" + itemCart, function (data) {
        $('.divMatHang').empty();
        $('.divMatHang').append(data);
        setTableScroll();
    });
}

function loadDataPag(url, divClass) {
    $.get(url, function (data) {
        $(divClass).empty();
        $(divClass).append(data);
        setTableScroll();
    });
}

function LocPhieuNhapKho() {
    let s = $(".ipSearch").val();
    let fDate = $(".dtTuNgay").val();
    let tDate = $(".dtDenNgay").val();
    let urlLoad = "/NhapKho/Index?page=1&s=" + s + "&fDate=" + fDate + "&tDate=" + tDate;
    window.location.href = urlLoad;
}

function setTableScroll() {
    let url = window.location.href;
    let heightSc = window.innerHeight;
    if (url.indexOf("MatHang") > -1) {
        heightSc -= $('.navbar').height() + $('.justify-content-end').height() + 100;
        $('.table-wrap').height(heightSc);
        $('body').css("overflow", "hidden");
        $('.divMatHang').find(".table-wrap").find("tr").each(function (i, e) {
            if ($('.divMatHang').find(".table-wrap").find("tr").length - 1 != i) $(e).css("height", "10px");
        });
    } else if (url.indexOf("NhapKho") > -1) {
        //fix table mặt hàng
        heightSc -= $('.navbar').height() + $('.ipSearch').height() + 120;
        $('.divMatHang').find(".table-wrap").height(heightSc);
        $('body').css("overflow", "hidden");
        $('.divMatHang').find(".table-wrap").find("tr").each(function (i, e) {
            if ($('.divMatHang').find(".table-wrap").find("tr").length - 1 != i) $(e).css("height", "10px");
        });
        //fix table details
        heightSc = window.innerHeight;
        heightSc -= $(".divTop").height() + $(".divFooter").height() + 93;
        $('.divDetails').find(".table_wrap").height(heightSc);
    }
}

$(document).ready(function () {
    let url = window.location.href;
    //css
    $('.table').find('tbody').find('tr').on("click", function () {
        $('.table').find('tbody').find('tr').each(function (i, e) {
            $(e).removeClass("table-active");
        });
        $(this).addClass("table-active");
    });

    //set height table
    setTableScroll();
    //end css

    //thêm
    $('.btnAdd').click(function () {
        if (url.indexOf("NhomMatHang") > -1) showNhomHang("");
        else if (url.indexOf("NhaCungCap") > -1) showNhaCungCap("");
        else if (url.indexOf("MatHang") > -1) showMatHang("");
    });
    //sửa
    $('.btnEdit').click(function () {
        let id = $(this).closest("tr").attr("data-id");
        if (url.indexOf("NhomMatHang") > -1) showNhomHang(id);
        else if (url.indexOf("NhaCungCap") > -1) showNhaCungCap(id);
        else if (url.indexOf("MatHang") > -1) showMatHang(id);
    });
    //xóa
    $('.btnDelete').click(function () {
        let id = $(this).closest("tr").attr("data-id");
        if (url.indexOf("NhomMatHang") > -1) deleteNhomHang(id);
        else if (url.indexOf("NhaCungCap") > -1) deleteNhaCungCap(id);
        else if (url.indexOf("MatHang") > -1) deleteMatHang(id);
        else if (url.indexOf("NhapKho") > -1) deleteNhapKho(id);
    });

    //Quản lý ảnh
    $('.btnUplloadFile').click(function () {
        let id = $(this).closest("tr").attr("data-id");
        if (url.indexOf("MatHang") > -1) {
            QuanLyAnhMatHang(id, $(this).closest("tr").find('td')[1].innerText);
        }
    });

    //lọc mặt hàng
    $('.tvMain').find("tbody").on("click", "tr", function () {
        LocMatHang($(this).attr("data-id"), $('.ipSearch').val());
    });

    $('.ipSearch').change(function () {
        //lọc mặt hàng
        if (url.indexOf("MatHang") > -1 || url.indexOf("NhapKho/AddOrUpdate") > -1) {
            let nhomId = "";
            $('.tvMain').find("tbody").find("tr").each(function (i, e) {
                if ($(e).hasClass("table-active")) {
                    nhomId = $(e).attr("data-id");
                }
            });
            LocMatHang(nhomId, $(this).val(), url.indexOf("MatHang") == -1);
        }
        else if (url.indexOf("NhapKho") > -1) {
            //lọc phiếu nhập kho
            LocPhieuNhapKho();
        }
    });

    $('.dtTuNgay').change(function () {
        //lọc phiếu nhập kho
        if (url.indexOf("NhapKho") > -1) {
            LocPhieuNhapKho();
        }
    });

    $('.dtDenNgay').change(function () {
        //lọc phiếu nhập kho
        if (url.indexOf("NhapKho") > -1) {
            LocPhieuNhapKho();
        }
    });

    $('.btnRefresh').click(function () {
        //lọc phiếu nhập kho
        if (url.indexOf("NhapKho") > -1) {
            LocPhieuNhapKho();
        }
    });

    $('body').on("click", ".page-link", function () {
        let data_url = $(this).attr("data-url");
        if (url.indexOf("MatHang") > -1 || url.indexOf("NhapKho") > -1) {
            loadDataPag(data_url, ".divMatHang");
        }
    });
});