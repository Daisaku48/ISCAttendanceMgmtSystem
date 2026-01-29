//2023-10-30 iwai-tamura upd-str ---
//初回 入力制御
$(window).on('load', function () {
    filterEmployeeList();
});

//2023-10-30 iwai-tamura upd-end ---


//対象者ドロップダウン選択時イベント
//2023-10-30 iwai-tamura upd-str ---
$('#Search_EmployeeData').on("change", function () {
    var selectedText = $("#Search_EmployeeData option:selected").text();
    $('#Search_EmployeeName').val(selectedText);
});
//$('#Search_EmployeeNo').on("change", function () {
//    var selectedText = $("#Search_EmployeeNo option:selected").text();
//    $('#Search_EmployeeName').val(selectedText);
//});
//2023-10-30 iwai-tamura upd-end ---

//2023-10-30 iwai-tamura upd-str ---
//日付変更時に入社・退職日を考慮し、社員リストを制御
$('#Search_EntryYear').on('change', filterEmployeeList);
$('#Search_EntryMonth').on('change', filterEmployeeList);

function filterEmployeeList() {
    // 検索年月を「yyyymm」形式の文字列に変換
    var dteSearchDateYear = $('#Search_EntryYear').val();
    var dteSearchDateMonth = $('#Search_EntryMonth').val();
    var yyyy = String(dteSearchDateYear.padStart(4, '0'));
    var mm = String(dteSearchDateMonth.padStart(2, '0'));
    var searchDateYM = '' + yyyy + mm;

    var selectedValue = $('#Search_EmployeeData').val(); // 選択されている値を取得
    var isSelectedValueVisible = false;

    // 検索日付時に勤続していない場合、リストに非表示
    $('#Search_EmployeeData option').each(function () {
        var value = $(this).val(); // リスト値の形式："[従業員Code],[入社年月日],[退職年月日]"
        var parts = value.split(',');
        var employmentStartYM = '';
        var employmentEndYM = '';
        if (value !== '') {
            if (parts[1] !== '') {
                employmentStartYM = parts[1].substring(0, 6);
            }
            if (parts[2] !== '') {
                employmentEndYM = parts[2].substring(0, 6);
            }
        }

        $(this).hide();
        if (parts[0] == 'ALL') {
            $(this).show();
            if (value === selectedValue) {
                isSelectedValueVisible = true;
            }
        } else if ((dteSearchDateYear !== '') && (dteSearchDateMonth !== '')) {
            if ((employmentStartYM <= searchDateYM)
                && ((employmentEndYM == '') || (employmentEndYM >= searchDateYM))) {
                //入社・退職年月日チェック
                $(this).show();
                if (value === selectedValue) {
                    isSelectedValueVisible = true;
                }            }
        }
    });

    // 選択されている項目が非表示の場合、選択を解除
    if (!isSelectedValueVisible) {
        $('#Search_EmployeeData').val('');
    }
};
//2023-10-30 iwai-tamura upd-end ---

$('#dmysearch').click(function () {

    //入力チェック
    var year = $('#Search_EntryYear').val();
    var month = $('#Search_EntryMonth').val();

    //2023-10-30 iwai-tamura upd-str ---
    var selectedText = $("#Search_EmployeeData option:selected").val();
    //var selectedText = $("#Search_EmployeeNo option:selected").val();
    //2023-10-30 iwai-tamura upd-str ---

    if (year.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象年月が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象年月が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }
    if (month.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象年月が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象年月が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    if (selectedText.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象者が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象者が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    if (selectedText == 'ALL') {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '全件の表示はできません。<br>確認してください。');
        //showMessage('注意', '全件の表示はできません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    //ボタンクリック
    $('#searchbutton').trigger('click');
});

$('#dmydatedownload').click(function () {
    //入力チェック
    var year = $('#Search_EntryYear').val();
    var month = $('#Search_EntryMonth').val();

    //2023-10-30 iwai-tamura upd-str ---
    var selectedText = $("#Search_EmployeeData option:selected").val();
    //var selectedText = $("#Search_EmployeeNo option:selected").val();
    //2023-10-30 iwai-tamura upd-str ---

    if (year.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象年月が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象年月が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }
    if (month.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象年月が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象年月が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    if (selectedText.length == 0) {
        //2023-11-30 iwai-tamura upd-str ------
        showAlert('注意', '対象者が指定されてません。<br>確認してください。');
        //showMessage('注意', '対象者が指定されてません。<br>確認してください。', false, false);
        //2023-11-30 iwai-tamura upd-end ------
        return;
    }

    //ボタンクリック
    $('#datedownloadbutton').trigger('click');
});


/*
 * ログアウトボタンクリック時
 */
$('#dmylogin').click(function () {
  //ボタンクリック
  showLogout(false, 'loginbutton');
});
