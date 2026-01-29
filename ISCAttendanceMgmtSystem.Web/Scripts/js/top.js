/*
 * ログアウトボタンクリック時
 */
$('#dmylogin').click(function () {
  //ボタンクリック
  showLogout(false, 'loginbutton');
});

//画面タイマー処理
window.onload = function () {
    Time();
}
function Time() {
    var realTime = new Date(Date.now() + ((new Date().getTimezoneOffset() + (9 * 60)) * 60 * 1000));

    var year = realTime.getFullYear();//年
    var mon = realTime.getMonth() + 1; //月 １を足す
    var day = realTime.getDate();//日

    // 数値が1桁の場合、頭に0を付けて2桁で表示する指定
    if (year < 10) { year = "0" + year; }
    if (mon < 10) { mon = "0" + mon; }
    if (day < 10) { day = "0" + day; }

    var you = realTime.getDay(); //曜日
    //曜日の配列（日～土）
    var wday = new Array("日", "月", "火", "水", "木", "金", "土");
    var Today = year + "/" + mon + "/" + day + "(" + wday[you] + ")";


    var hour = realTime.getHours();
    var minutes = realTime.getMinutes();
    var seconds = realTime.getSeconds();

    // 数値が1桁の場合、頭に0を付けて2桁で表示する指定
    if (hour < 10) { hour = "0" + hour; }
    if (minutes < 10) { minutes = "0" + minutes; }
    if (seconds < 10) { seconds = "0" + seconds; }

    var text = hour + ":" + minutes + ":" + seconds;
    document.getElementById("Today").innerHTML = Today;
    document.getElementById("Time").innerHTML = text;
    document.getElementById("TopView_StandardDate").value = Today;
    document.getElementById("TopView_StandardTime").value = text;
    document.getElementById("TopView_StandardDateTime").value = year + "/" + mon + "/" + day + " " + hour + ":" + minutes + ":" + seconds;
}
setInterval('Time()', 1000);


//2023-02-08 iwai-tamura upd-str ------
/*
 * 変更ボタンクリック時
 */
$('#dmyFinish').click(function () {
    //2023-10-30 iwai-tamura upd-str ------
    //画面
    var viewStartTimeElement = document.getElementById("viewStartTime");
    var viewStartTimeValue = viewStartTimeElement.textContent || viewStartTimeElement.innerText;

    if (viewStartTimeValue === null || viewStartTimeValue.trim() === "") {
        showMessage('確認', '本日の出勤処理がされておりません。このまま退勤しますか？', 'finish', false);
    } else {
        showMessage('確認', '退勤しますか？', 'finish', false);
    }
    //showMessage('確認', '退勤しますか？', 'finish', false);
    //2023-10-30 iwai-tamura upd-end ------
});
//2023-02-08 iwai-tamura upd-end ------

//2023-10-30 iwai-tamura upd-str ------
$('#dmyStatus').click(function () {
    //画面
    showStatusSelect('ステータス選択', 'ステータスを選択してください。', false, false,true);
});

function showStatusSelect(title, message, buttonid, isLoding,isCloseBtn) {

    // ダイアログのメッセージを設定
    $('#show_dialog').html(message);

    // ダイアログを作成
    $('#show_dialog').dialog({
        dialogClass: 'vertical-buttons',
        modal: true,
        title: title,

        closeOnEscape: isCloseBtn,  // エスケープキーでの閉じる動作を無効化
        closeText: '',         // 閉じるボタンのテキストを空にする（アイコンだけの場合）

        buttons: [
            {
                text: '出社',
                class: 'button-attend',
                click: function () {
                    $(this).dialog('close');
                    if (isLoding) {
                        showLoading();
                    }
                    if (isCloseBtn) {
                        $('#change_status_attend').trigger('click');

                    } else {
                        $('#change_status_attend_begin').trigger('click');
                    }
                }
            },
            {
                text: 'テレワーク',
                class: 'button-telework',
                click: function () {
                    $(this).dialog('close');
                    if (isLoding) {
                        showLoading();
                    }
                    if (isCloseBtn) {
                        $('#change_status_telework').trigger('click');

                    } else {
                        $('#change_status_telework_begin').trigger('click');
                    }
                }
            },
            {
                text: '外出',
                class: 'button-out',
                click: function () {
                    $(this).dialog('close');
                    if (isLoding) {
                        showLoading();
                    }
                    if (isCloseBtn) {
                        $('#change_status_out').trigger('click');

                    } else {
                        $('#change_status_out_begin').trigger('click');
                    }
                }
            }
        ],
        open: function (event, ui) {
            //出勤ボタン・ステータス変更ボタンによって制御
            if (isCloseBtn) {
                //ステータスボタン
                $("#your-dialog-id .ui-dialog-titlebar-close").show();  //ステータスボタン時は右上の閉じるボタンを有効化
                $(this).parent().css('width', '300px');
            } else {
                $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();  //出勤ボタン時は右上の閉じるボタンを無効化
                $(this).parent().css('width', '350px');
            }

        }
    });
}
//2023-10-30 iwai-tamura upd-end ------


