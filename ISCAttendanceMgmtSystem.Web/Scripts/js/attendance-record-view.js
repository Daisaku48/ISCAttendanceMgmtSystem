$('#dmysearch').click(function () {

    //入力チェック
    var year = $('#Search_EntryYear').val();
    var month = $('#Search_EntryMonth').val();
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

    //ボタンクリック
    $('#searchbutton').trigger('click');
});

$('#dmydatedownload').click(function () {
    //入力チェック
    var year = $('#Search_EntryYear').val();
    var month = $('#Search_EntryMonth').val();
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
