//2022-99-99 iwai-shibuya add-str ------ ここでパスワードチェックをいれる
/*
 * 変更ボタンクリック時
 */
$('#dmychange').click(function () {
    var oldpw = $('#OldPassword').val();
    var newpw = $('#NewPassword').val();
    var confipw = $('#ConfirmPassword').val();

    if (oldpw.length == 0) {
        showMessage('注意', '現在のパスワードが入力されていません。<br>確認してください。', false, false);
        return;
    }

    if (newpw.length == 0) {
        showMessage('注意', '新しいパスワードが入力されていません。<br>確認してください。', false, false);
        return;
    }

    if (confipw.length == 0) {
        showMessage('注意', '確認用パスワードが入力されていません。<br>確認してください。', false, false);
        return;
    }

    //文字数チェック
    if (newpw.length < 8) {
        showMessage('注意', 'パスワードは8文字以上にしてください。<br>確認してください。', false, false);
        return;
    }

    //パスワードの複雑さチェック
    let strength = 0    //複雑さ
    // 英字の大文字を含んでいれば+1
    if (newpw.match(/([a-z])/)) strength += 1
    // 英字の大文字を含んでいれば+1
    if (newpw.match(/([A-Z])/)) strength += 1
    // 数字を含んでいれば+1
    if (newpw.match(/([0-9])/)) strength += 1
    // 記号を含んでいれば+1
    if (newpw.match(/([!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1

    if (strength < 3) {
        showMessage('注意', 'パスワードの要件をみたしておりません。<br>確認してください。', false, false);
        return;
    }

    //2023-10-30 iwai-tamura add-str ---
    //新旧パスワード相違チェック
    if (oldpw == newpw) {
        showMessage('注意', '旧パスワードと新パスワードが同じです。<br>確認してください。', false, false);
        return;
    }
    //2023-10-30 iwai-tamura add-end ---


    if (newpw != confipw) {
        showMessage('注意', '新しいパスワードと確認のパスワードが一致しません。<br>確認してください。', false, false);
        //$('#test').text('パスワードが一致しません');
        return;
    }

    showMessage('変更確認', '変更しますか？', 'changebutton', false);
});
//2022-99-99 iwai-shibuya add-end ------
