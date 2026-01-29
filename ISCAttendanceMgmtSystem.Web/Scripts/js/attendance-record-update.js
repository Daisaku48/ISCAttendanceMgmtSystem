
$(function () {
    //初回 入力制御
    //datepicker設定
    $.datepicker.setDefaults($.datepicker.regional['ja']);
    $('.ymd-control').datepicker();

    if ($('#Head_ViewType').val() == '1') {
        $(".save-input").attr('disabled', true);
    } else if ($('#Head_ViewType').val() == '2') {
        $(".search-input").attr('disabled', true);
    }
    //2023-10-30 iwai-tamura upd-str ---
    $(".attendancetime-input").attr('disabled', true);
    $(".status-input").attr('disabled', true);
    $(".comment-input").attr('disabled', true);

    //初期処理
    $('#Search_Date').trigger('change');

    //2023-10-30 iwai-tamura upd-end ---
});


//2023-10-30 iwai-tamura upd-str ---
//エンターキー無効
$(function () {
    $("input").on("keydown", function (e) {
        if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
            return false;
        } else {
            return true;
        }
    });
});

//処理区分変更時
$(document).ready(function () {
    // 初期値保持
    originalBeginHour = $('#Input_InputBeginHour').val();
    originalBeginMinutes = $('#Input_InputBeginMinutes').val();
    originalFinishHour = $('#Input_InputFinishHour').val();
    originalFinishMinutes = $('#Input_InputFinishMinutes').val();
    originalAttendanceStatus = $('#attendanceStatusDropdown').val();


    // Input.UpdateTypeという名前のラジオボタンが変更されたときに発火
    $('input[name="Input.UpdateType"]').change(function () {
        $('#Input_InputBeginHour').val(originalBeginHour);
        $('#Input_InputBeginMinutes').val(originalBeginMinutes);
        $('#Input_InputFinishHour').val(originalFinishHour);
        $('#Input_InputFinishMinutes').val(originalFinishMinutes);
        $('#attendanceStatusDropdown').val(originalAttendanceStatus);

        if ($(this).val() == '20') {            //出勤修正
            $(".attendancetime-input").attr('disabled', false);
            $(".status-input").attr('disabled', true);
            $(".comment-input").attr('disabled', false);
        } else if ($(this).val() == '21') {     //ステータス修正
            $(".attendancetime-input").attr('disabled', true);
            $(".status-input").attr('disabled', false);
            $(".comment-input").attr('disabled', false);
        } else if ($(this).val() == '22') {     //出勤&ステータス修正
            $(".attendancetime-input").attr('disabled', false);
            $(".status-input").attr('disabled', false);
            $(".comment-input").attr('disabled', false);
        } else {     //未選択
            $(".attendancetime-input").attr('disabled', true);
            $(".status-input").attr('disabled', true);
            $(".comment-input").attr('disabled', true);
        }

        //ステータスリスト制御
        if ($(this).val() == '22') {     //出勤&ステータス修正
            $("#attendanceStatusDropdown option[value='91']").remove(); //休みを選択不可とする
        } else {
            //上記以外の時に「休み」を復活させる
            if ($("#attendanceStatusDropdown option[value='91']").length == 0) { //「休み」オプションが存在しない場合のみ追加
                $("#attendanceStatusDropdown").append(new Option("休み", "91"));
            }
        }

    });
});


//日付変更時に入社・退職日を考慮し、社員リストを制御
$('#Search_Date').on('change', function () {

    // 検索日付を「yyyymmdd」形式の文字列に変換
    var dteSearchDate = $(this).val();
    var aryDate = dteSearchDate.split('/');
    var yyyy = String(aryDate[0]).padStart(4, '0');
    var mm = String(aryDate[1]).padStart(2, '0');
    var dd = String(aryDate[2]).padStart(2, '0');
    var searchDate = '' + yyyy + mm + dd;

    //2024-09-10 iwai-tamura add-str ---
    var isSelectedValueVisible = false;
    //2024-09-10 iwai-tamura add-end ---

    // 検索日付時に勤続していない場合、リストに非表示
    $('#Search_EmployeeData option').each(function () {
        var value = $(this).attr('value');  //リスト値の形式："[従業員Code],[入社年月日],[退職年月日]"
        var parts = value.split(',');
        var employmentStartDate = parts[1];
        var employmentEndDate = parts[2];

        //2024-09-10 iwai-tamura add-str ---
        var selectedValue = $('#Search_EmployeeData').val(); // 選択されている値を取得
        //2024-09-10 iwai-tamura add-end ---


        $(this).show();

        //2024-09-10 iwai-tamura upd-str ---
        if (employmentStartDate > searchDate) {
            //入社年月日チェック
            $(this).hide();
        } else if (employmentEndDate !== "") {
            //退職年月日チェック
            if (employmentEndDate < searchDate) {
                $(this).hide();
            } else {
                if (value === selectedValue) {
                    isSelectedValueVisible = true;
                }
            }
        } else {
            if (value === selectedValue) {
                isSelectedValueVisible = true;
            }
        }
        //if (employmentStartDate > searchDate) {
        //    //入社年月日チェック
        //    $(this).hide();
        //} else if (employmentEndDate !== "") {
        //    //退職年月日チェック
        //    if (employmentEndDate < searchDate) {
        //        $(this).hide();
        //    }
        //}
        //2024-09-10 iwai-tamura upd-end ---
    });

    //2024-09-10 iwai-tamura add-str ---
    if (!isSelectedValueVisible) {
        $('#Search_EmployeeData').val('');
    }
    //2024-09-10 iwai-tamura add-end ---
});
//2023-10-30 iwai-tamura upd-end ---


$('#dmysearch').click(function () {

    //入力チェック
    var SearchDate = $('#Search_Date').val();
    var SearchNo = $('#Search_EmployeeData').val();
    if (SearchDate.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象の日付が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象の日付が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }
    if (SearchNo.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象従業員が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象従業員が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    //ボタンクリック
    $('#searchbutton').trigger('click');
});

$('#dmysave').click(function () {

    //入力チェック
    //2023-10-30 iwai-tamura upd-str ------
    var UpdateType = $('input[name="Input.UpdateType"]:checked').val();
    var AttendanceStatus = $('#attendanceStatusDropdown').val();
    //2023-10-30 iwai-tamura upd-end ------
    var BeginHour = $('#Input_InputBeginHour').val();
    var BeginMinutes = $('#Input_InputBeginMinutes').val();
    var FinishHour = $('#Input_InputFinishHour').val();
    var FinishMinutes = $('#Input_InputFinishMinutes').val();
    var Comment = $('#Input_InputComment').val();

    //2023-10-30 iwai-tamura upd-str ------
    //修正内容入力チェック
    if (!UpdateType) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '修正内容が選択されていません。<br>確認してください。');
        //showMessage('注意', '修正内容が選択されていません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }
    //2023-10-30 iwai-tamura upd-end ------

    //時、分片方のみ入力は不正
    if ((BeginHour.length == 0 && BeginMinutes.length == 0) || (BeginHour.length != 0 && BeginMinutes.length != 0)) {
    } else {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '出勤時間が不正です。<br>確認してください。');
        //showMessage('注意', '出勤時間が不正です。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }
    if ((FinishHour.length == 0 && FinishMinutes.length == 0) || (FinishHour.length != 0 && FinishMinutes.length != 0)) {
    } else {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '退勤時間が不正です。<br>確認してください。');
        //showMessage('注意', '退勤時間が不正です。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    //2023-10-30 iwai-tamura upd-str ------
    // 出勤時間と退勤時間が逆転しているかのチェック
    if (BeginHour.length != 0 && BeginMinutes.length != 0 && FinishHour.length != 0 && FinishMinutes.length != 0) {
        var beginTime = parseInt(BeginHour) * 60 + parseInt(BeginMinutes); // 出勤時間を分単位で変換
        var finishTime = parseInt(FinishHour) * 60 + parseInt(FinishMinutes); // 退勤時間を分単位で変換

        if (beginTime >= finishTime) {
            //2023-11-30 iwai-tamura upd-str ------
            showAlert('注意', '出勤時間と退勤時間が逆転しています。<br>確認してください。');
            //showMessage('注意', '出勤時間と退勤時間が逆転しています。<br>確認してください。', false, false);
            //2023-11-30 iwai-tamura upd-end ------
            return;
        }
    }

    //出勤、退勤共に入力されていない場合はエラー
    //ステータスのみ変更時はOK（休みの時等）
    if (UpdateType != "21") {
        if (BeginHour.length == 0 && BeginMinutes.length == 0 && FinishHour.length == 0 && FinishMinutes.length == 0) {
            //2023-11-30 iwai-tamura upd-str ------
            showAlert('注意', '勤務データがありません。<br>確認してください。');
            //showMessage('注意', '勤務データがありません。<br>確認してください。', false, false);
            //2023-11-30 iwai-tamura upd-end ------
            return;
        }
    }
    //if (BeginHour.length == 0 && BeginMinutes.length == 0 && FinishHour.length == 0 && FinishMinutes.length == 0) {
    //    showMessage('注意', '勤務データがありません。<br>確認してください。', false, false);
    //    return;
    //}
    //2023-10-30 iwai-tamura upd-end ------


    //2023-10-30 iwai-tamura upd-str ------
    //ステータス入力チェック
    if (!AttendanceStatus) {
        if (UpdateType == "20") {   //時間のみ変更時
            //2023-11-30 iwai-tamura upd-str ------
            showAlert('注意', 'ステータスが選択されていません。<br>確認してください。');
            //showMessage('注意', 'ステータスが選択されていません。<br>確認してください。', false, false);
            //2023-11-30 iwai-tamura upd-end ------
            return;
        } else {
            //2023-11-30 iwai-tamura upd-str ------
            showAlert('注意', 'ステータスが選択されていません。<br>確認してください。');
            //showMessage('注意', 'ステータスが選択されていません。<br>確認してください。', false, false);
            //2023-11-30 iwai-tamura upd-end ------
            return;

        }
    }

    //退勤時間を入力した場合、必ずステータスは退勤
    if (!(FinishHour.length == 0 && FinishMinutes.length == 0)) {
        if (!(AttendanceStatus=="81")) {
            //2023-11-30 iwai-tamura upd-str ------
            showAlert('注意', '退勤時間が入力されている場合は、ステータスを退勤済みにしてください。');
            //showMessage('注意', '退勤時間が入力されている場合は、ステータスを退勤済みにしてください。', false, false);
            //2023-11-30 iwai-tamura upd-end ------
            return;
        }    
    }
    //2023-10-30 iwai-tamura upd-end ------


    //2024-09-10 iwai-tamura add-str ------
    //未来日の修正を制限(休み等の入力以外は不可)
    //修正内容ステータス。ステータス休みの場合おｋ

    var SearchDate = $('#Search_Date').val();   //登録日取得

    // SearchDateをDateオブジェクトに変換
    var searchDateObj = new Date(SearchDate.replace(/-/g, '/')); // replaceで日付フォーマットが一致しない場合に対応

    // 現在の日付を取得（時間部分を0に設定）
    var today = new Date();
    today.setHours(0, 0, 0, 0); // 時間部分を削除して日付のみを比較する

    // SearchDateが未来日か判定
    if (searchDateObj > today) {
        // 未来日のデータの登録は休みのみ可能とする。
        if (!(AttendanceStatus == "91")||(UpdateType != "21")) {
            showAlert('注意', '未来日の修正は休みの登録のみ行えます。');
            return;
        }
    }
    //2024-09-10 iwai-tamura add-end ------


    //コメント文字数チェック
    //2023-02-28 iwai-tamura upd-str ------
    if (Comment.trim().length == 0) {
    //if (Comment.length == 0) {
    //2023-02-28 iwai-tamura upd-end ------
        //2023-11-30 iwai-tamura upd-str ------
        //2023-02-08 iwai-tamura upd-str ------
        showAlert('注意', '事由が記載されておりません。<br>確認してください。');
        //showMessage('注意', '事由が記載されておりません。<br>確認してください。', false, false);
        ////showMessage('注意', '備考が記載されておりません。<br>確認してください。', false, false);
        //2023-02-08 iwai-tamura upd-end ------
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }
    if (Comment.length > 60) {
        //2023-11-30 iwai-tamura upd-str ------
        //2023-02-08 iwai-tamura upd-str ------
        showAlert('注意', '事由の文字数が多すぎです。<br>確認してください。(最大60文字)');
        //showMessage('注意', '事由の文字数が多すぎです。<br>確認してください。(最大60文字)', false, false);
        ////showMessage('注意', '備考の文字数が多すぎです。<br>確認してください。(最大60文字)', false, false);
        //2023-02-08 iwai-tamura upd-end ------
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    //ボタンクリック
    showMessage('変更確認', '勤務実績を修正します。<br>よろしいですか？', 'savebutton', false);
    //$('#savebutton').trigger('click');
});

$('#dmycancel').click(function () {
    //ボタンクリック
    $('#cancelbutton').trigger('click');
});


//保存直前にdisabled解除(値が送られないため)
function removeDisabled() {
    $('input').each(function (i, elem) {
        elem.disabled = false;
    });
    $('select').each(function (i, elem) {
        elem.disabled = false;
    });
}


//日付削除ボタン
$('button.input-control').click(function () {
    $('#' + this.value).val('');
});




/*
 * ログアウトボタンクリック時
 */
$('#dmylogin').click(function () {
  //ボタンクリック
  showLogout(false, 'loginbutton');
});
