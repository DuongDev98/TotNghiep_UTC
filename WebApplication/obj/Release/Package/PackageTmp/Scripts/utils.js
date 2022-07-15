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
        else if (urlSubmit.indexOf("HoaDon/CapNhatImei") > -1)
        {
            let formData = null;
            let imei = $('#modal-dialog').find(".ipImei").val();
            $.post(urlSubmit + '?imei=' + imei, formData, (success) => {
                if (success.length == 0) {
                    $('#modal-dialog').find(".alert-danger").attr("hidden", "hidden");
                    location.reload();
                } else {
                    $('#modal-dialog').find(".alert-danger").removeAttr("hidden");
                    $('#modal-dialog').find(".alert-danger").text(success);
                }
            });
        }
        else if (urlSubmit.indexOf("HoaDon/CapNhatTrangThai") > -1) {
            let formData = null;
            let trangThai = $('#modal-dialog').find(".ipTrangThai").val();
            $.post(urlSubmit + '?trangThai=' + trangThai, formData, (success) => {
                if (success.length == 0) {
                    $('#modal-dialog').find(".alert-danger").attr("hidden", "hidden");
                    location.reload();
                } else {
                    $('#modal-dialog').find(".alert-danger").removeAttr("hidden");
                    $('#modal-dialog').find(".alert-danger").text(success);
                }
            });
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

//Thương hiệu
function showThuongHieu(id) {
    $.get("/ThuongHieu/Item/" + id, function (data) {
        ShowAddEditForm(id.length == 0 ? "Thêm mới thương hiệu" : "Chỉnh sửa thương hiệu", data, "/ThuongHieu/AddOrEdit/" + id);
    });
}

function deleteThuongHieu(id) {
    ShowYesNo("Lựa chọn", "Bạn muốn xóa nhóm mặt hàng đang chọn?", () => {
        $.post("/ThuongHieu/Delete/" + id, function (data) {
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

//function QuanLyAnhMatHang(id, name) {
//    let html = '<form id="data" method="post" enctype="multipart/form-data"><div class="input-images"></div></form>';
//    ShowAddEditForm(name, html, "/MatHang/UploadAnh/" + id);
//    $.get("/MatHang/GetImage/" + id, function (dataStr) {
//        let preloaded = [];
//        if (dataStr != "[]") {
//            let dataArr = JSON.parse(dataStr);
//            for (let i = 0; i < dataArr.length; i++) {
//                let item = dataArr[i];
//                preloaded.push({ id: item.ID, src: item.LINK });
//            }
//        }
//        $('.input-images').imageUploader({
//            preloaded: preloaded
//        });
//    });
//}

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

function LocMatHang(nhomId, thuongHieuId, search, itemCart) {
    $.get("/MatHang/Table?page=1&nhomId=" + nhomId + "&thuongHieuId=" + thuongHieuId + "&s=" + search + "&itemCart=" + itemCart, function (data) {
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
        //$('body').css("overflow", "hidden");
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
        //heightSc = window.innerHeight;
        //heightSc -= $(".divTop").height() + $(".divFooter").height() + 93;
        //$('.divDetails').find(".table_wrap").height(heightSc);

        heightSc = window.innerHeight;
        heightSc -= $(".divTop").height() + 93;
        $('.divDetails').find(".table_wrap").css("max-height", heightSc + "px").css("overflow", "auto");
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

    $('.table-hover').find("thead").css("background-color", "whitesmoke");

    //set height table
    setTableScroll();
    //end css

    //thêm
    $('.btnAdd').click(function () {
        if (url.indexOf("NhomMatHang") > -1) showNhomHang("");
        if (url.indexOf("ThuongHieu") > -1) showThuongHieu("");
        else if (url.indexOf("NhaCungCap") > -1) showNhaCungCap("");
        else if (url.indexOf("MatHang") > -1) showMatHang("");
    });
    //sửa
    $('.btnEdit').click(function () {
        let id = $(this).closest("tr").attr("data-id");
        if (url.indexOf("NhomMatHang") > -1) showNhomHang(id);
        if (url.indexOf("ThuongHieu") > -1) showThuongHieu(id);
        else if (url.indexOf("NhaCungCap") > -1) showNhaCungCap(id);
        else if (url.indexOf("MatHang") > -1) showMatHang(id);
    });
    //xóa
    $('.btnDelete').click(function () {
        let id = $(this).closest("tr").attr("data-id");
        if (url.indexOf("NhomMatHang") > -1) deleteNhomHang(id);
        if (url.indexOf("ThuongHieu") > -1) deleteThuongHieu(id);
        else if (url.indexOf("NhaCungCap") > -1) deleteNhaCungCap(id);
        else if (url.indexOf("MatHang") > -1) deleteMatHang(id);
        else if (url.indexOf("NhapKho") > -1) deleteNhapKho(id);
    });

    //Quản lý ảnh
    //$('.btnUplloadFile').click(function () {
    //    let id = $(this).closest("tr").attr("data-id");
    //    if (url.indexOf("MatHang") > -1) {
    //        QuanLyAnhMatHang(id, $(this).closest("tr").find('td')[1].innerText);
    //    }
    //});

    //lọc mặt hàng
    $('.tvMain').find("tbody").on("click", "tr", function () {
        LocMatHang($(this).attr("data-id"), "", $('.ipSearch').val(), "");
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
            LocMatHang(nhomId, "", $(this).val(), url.indexOf("MatHang") == -1);
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

    $(".divMatHang").on("click", "tr", function () {
        if (url.indexOf("NhapKho") == -1) return;
        let id = $(this).data("id");
        let code = $(this).data("code");
        let name = $(this).data("name");
        let giaNhap = convertToInt($(this).data("gianhap"));
        let coImei = $(this).data("coimei") == "30";
        //Tìm xem trong chi tiết có không, có thì cộng thêm, không có thì
        if (coImei) {
            //Nhập form nhập imei
            let title = "Nhập danh sách imei";
            let html = "<textarea id='txtImei' rows='5' style='width: 100%'></textarea>";
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
                '           <div class="alert alert-danger" role="alert" hidden="hidden"></div >' +
                '         ' + html + '    ' +
                '      </div>\n' +
                '   <div class="modal-footer">' +
                '       <button type="button" id="btnYes" class="btn btn-success">Chấp nhận</button>' +
                '       <button type="button" class="btn btn-danger" data-dismiss="modal">Thoát</button>' +
                '   </div>' +
                '    </div>\n' +
                '</div>');
            $('#modal-dialog').modal('show');
            $('#modal-dialog').on("click", "#btnYes", function (e) {
                e.preventDefault();
                $('#modal-dialog').modal('hide');
                let txt = $('#modal-dialog').find("#txtImei").val();
                let lines = txt.split("\n");
                lines.forEach((val, index) => {
                    if (val.length > 0) addItemToNhapKho(id, code, name, giaNhap, coImei, val);
                });
            });
        }
        else {
            //kiểm tra có tồn tại chưa, chưa thì thêm mới, không thì cộng số lượng
            addItemToNhapKho(id, code, name, giaNhap, coImei, "");
        }
    });

    function convertToInt(s) {
        let val = 0;
        if (s == null || s == undefined) return val;
        s = s.replace(",", "");
        val = parseInt(parseFloat(s).toFixed(0));
        return val;
    }

    function addItemToNhapKho(id, code, name, gianhap, coImei, imei) {
        let added = false;
        if (!coImei) {
            $('.tblChiTiet').find('tbody').find("tr").each((index, ele) => {
                if (id == ele.dataset.id) {
                    added = true;
                    let val = convertToInt($(ele).find("td:eq(4)").find("input").val()) + 1;
                    $(ele).find("td:eq(4)").find("input").val(val);
                }
            });
        }
        if (!added) {
            $tr = $('<tr data-id="' + id + '">' +
                '        <td>#</td>' +
                '        <td><input type="text" value="' + id + '" name="" hidden="hidden"/>' + code+'</td>' +
                '        <td>' + name+'</td>' +
                '        <td><input type="number" value="' + gianhap + '" name="" readonly/></td>' +
                '        <td><input type="number" value="1" name="" readonly/></td>' +
                //'        <td>0</td>' +
                //'        <td>0</td>' +
                '        <td>' + 1 * gianhap + '</td>' +
                '        <td><input type="text" value="' + imei + '" name="" hidden="hidden"/>' + imei + '</td>' +
                '        <td><div class="btn btn-danger">Xóa</div></td>' +
                '    </tr>');
            $('.tblChiTiet').find('tbody').append($tr);
        }
        CalculateRow();
    }

    $('.tblChiTiet').on("click", ".btn-danger", function () {
        if (url.indexOf("NhapKho") > -1) {
            $(this).closest("tr").remove();
        }
        CalculateRow();
    });

    $('#numTiLeGiam').change(function () {
        CalculateRow();
    });

    function CalculateRow() {
        let tienHang = 0;
        let tileGiam = convertToInt($("#numTiLeGiam").val());
        let tienGiamGia = 0;
        let tongCong = 0;
        //3 don gia, 4 so luong, 5 thanh tien
        $('.tblChiTiet').find('tbody').find("tr").each(function (index, ele) {
            let dongia = convertToInt($(ele).find("td:eq(3)").find("input").val());
            let soluong = convertToInt($(ele).find("td:eq(4)").find("input").val());
            let thanhtien = dongia * soluong;
            tienHang += thanhtien;
            $(ele).find("td:eq(5)").val(thanhtien);
            //find field
            $(ele).find("td:eq(0)").text(index);
            $(ele).find("td:eq(1)").find("input:eq(0)").attr("name","TDONHANGCHITIETs[" + index + "].DMATHANGID");
            $(ele).find("td:eq(3)").find("input").attr("name","TDONHANGCHITIETs[" + index + "].DONGIA");
            $(ele).find("td:eq(4)").find("input").attr("name","TDONHANGCHITIETs[" + index + "].SOLUONG");
            $(ele).find("td:eq(5)").find("input").attr("name","TDONHANGCHITIETs[" + index + "].THANHTIEN");
            $(ele).find("td:eq(6)").find("input").attr("name","TDONHANGCHITIETs[" + index + "].IMEI");
        });
        tienGiamGia = tienHang * (tileGiam / 100);
        tongCong = tienHang - tienGiamGia;
        $("#numTienHang").val(tienHang);
        $("#numTiLeGiam").val(tileGiam);
        $("#numTienGiam").val(tienGiamGia);
        $("#numTongCong").val(tongCong);
    }

    $('tbody').on("click", '#btnNhapImei', function () {
        let dataId = $(this).attr("data-id");
        let html = '<div><input class="form-control ipImei" type="text" value="" name="IMEI"/></div>';
        ShowAddEditForm("Nhập Imei bán hàng", html, "/HoaDon/CapNhatImei/" + dataId);
    });

    $('.row').on("click", '.btnDoiTrangThai', function () {
        let dataId = $(this).attr("data-id");
        let html = '<select class="custom-select ipTrangThai">' +
                        '<option value="0">Chờ xử lý</option>' +
                        '<option value="1">Đã hủy</option>' +
                        '<option value="2">Đang giao hàng</option>' +
                        '<option value="3">Đã nhận hàng</option>'+
                        '<option value="4">Đã thanh toán</option>'+
                  '</select>';
        ShowAddEditForm("Chọn trạng thái", html, "/HoaDon/CapNhatTrangThai/" + dataId);
    });

    $('#btnNhapKho').click(function () {
        //kiểm tra xem có dữ liệu chưa
        var form = $(this).closest('form');
        var tbody = form.find(".tblChiTiet").find("tbody");
        if (tbody.find("tr").length == 0) {
            alert("Dữ liệu chi tiết trống");
        }
        else {
            var arr = [];
            tbody.find("tr").each((index, element) => {
                var imei = $(element).find("td:eq(6)").text();
                if (imei.length > 0) arr.push(imei);
            });
            if (arr.length > 0) {
                var TDONHANGID = form.find("#ipID").val();
                $.post("/NhapKho/KiemTraTrungImeiNhapKho", { TDONHANGID: TDONHANGID, attrs: arr }, function (data) {
                    if (data.length > 0) alert(data);
                    else {
                        form.submit();
                    }
                });
            }
            else
            {
                form.submit();
            }
        }
    });
});