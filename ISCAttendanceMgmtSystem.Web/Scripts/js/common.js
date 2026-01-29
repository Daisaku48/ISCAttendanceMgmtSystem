/*
 * ダイアログメッセージ表示(OkCancel)
 * @param {string} title タイトル
 * @param {string} message メッセージ
 * @param {string} buttonid ボタンID
 * @param {bool} isLoding ローディングの要否
 */
function showMessage(title, message, buttonid, isLoding) {

    // ダイアログのメッセージを設定
    $('#show_dialog').html(message);

    // ダイアログを作成
    $('#show_dialog').dialog({
        modal: true,
        title: title,
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                //document.getElementById('myform').submit();
                if (isLoding) {
                    //scroll(0, 0);
                    //var outerPane = document.getElementById('loadingPanel');
                    //var innerPane = document.getElementById('loadingMessage');
                    //if (outerPane) outerPane.className = 'loading-panel-on';
                    //setTimeout('document.getElementById('waitImage').src='../../Content/img/726.GIF';', 10);
                    showLoading();
                }
                $('#' + buttonid).trigger('click');
            },
            'キャンセル': function () {
                $(this).dialog('close');
                return;
            }
        },
        open: function () {						// キャンセルボタンにフォーカスをあてる
            $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
        }
    });
}

/*
 * ダイアログメッセージ表示(OK)
 * @param {string} title タイトル
 * @param {string} message メッセージ
 */
function showAlert(title, message) {
    // ダイアログのメッセージを設定
    $('#show_dialog').html(message);

    // ダイアログを作成
    $('#show_dialog').dialog({
        modal: true,
        title: title,
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                return;
            }
        }
    });
}

/*
 * ローディング表示
 */
function showLoading() {
    scroll(0, 0);
    $("body").css("overflow", "hidden");
    var outerPane = document.getElementById('loadingPanel');
    var innerPane = document.getElementById('loadingMessage');
    if (outerPane) outerPane.className = 'loading-panel-on';

    setTimeout("document.getElementById('waitImage').src='../Content/img/726.GIF';", 3000);
}

/*
 * ログアウトダイアログ表示
 * @param {bool} isNavi ナビの要否
 * @param {string} buttonid ボタンID
 */
function showLogout(isNavi, buttonid) {
    // ダイアログのメッセージを設定
    $('#show_dialog').html('ログアウトしますか？');

    // ダイアログを作成
    $('#show_dialog').dialog({
        modal: true,
        title: 'ログアウト確認',
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                if (isNavi) {
                    document.getElementById('logoutForm').submit();
                } else {
                    $('#' + buttonid).trigger('click');
                }
            },
            'キャンセル': function () {
                $(this).dialog('close');
                return;
            }
        },
        open: function () {						// キャンセルボタンにフォーカスをあてる
            $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
        }
    });
}

/*
 * エスケープ
 * @param {string} val 文字列
 * @returns {string} エスケープした文字列
 */
function selectorEscape(val) {
  return val.replace(/[ !"#$%&'()*+,.\/:;<=>?@\[\\\]^`{|}~]/g, '\\$&');
}

/*
 * 日付チェック
 * @param {string} year 年
 * @param {string} year 月
 * @param {string} year 日
 * @returns {number} 0・・・正常、1・・・日付エラー、2～4・・半角エラー年月日
 */
function isExistDate(year, mon, day) {

  //未入力時はチェックなし
  if (year === '' && mon === '' && day === '') {
    return 0;
  }

  //全入力済みか？
  if (year !== '' && mon !== '' && day !== '') {
    if (year.match(/[^0-9]+/)) {
      //year.focus()
      return 2;
    }
    if (mon.match(/[^0-9]+/)) {
      //mon.focus()
      return 3;
    }
    if (day.match(/[^0-9]+/)) {
      //day.focus()
      return 4;
    }
    var iy = parseInt(year);
    var im = parseInt(mon);
    var id = parseInt(day);
    var date = new Date(iy, im - 1, id);
    if (iy !== date.getFullYear() ||
        im !== (date.getMonth() + 1) ||
        id !== date.getDate()) {
      //year.focus()
      //mon.focus()
      return 1;
    }
  } else {
    //year.focus()
    //mon.focus()
    return 1;
  }

  return 0;
}

/*
 * キー制御
 */
$(function(){
  // return値falseでキャンセル
  $(document).keydown(function(event){
    // キーコード
    var keyCode = event.keyCode;
    // Ctrl
    var ctrlClick = event.ctrlKey;
    // Alt
    var altClick = event.altKey;
    // キーイベント
    var obj = event.target;

    // ファンクションキー制御
    if(keyCode>=112 && keyCode<=123) {
      //F1-F12
      return false;
    }
    //if(keyCode == 112 // F1キーの制御
    //	|| keyCode == 113 // F2キーの制御
    //	|| keyCode == 114 // F3キーの制御
    //	|| keyCode == 115 // F4キーの制御
    //	|| keyCode == 116 // F5キーの制御
    //	|| keyCode == 117 // F6キーの制御
    //	|| keyCode == 118 // F7キーの制御
    //	|| keyCode == 119 // F8キーの制御
    //	|| keyCode == 120 // F9キーの制御
    //	|| keyCode == 121 // F10キーの制御
    //	|| keyCode == 122 // F11キーの制御
    //	|| keyCode == 123 // F12キーの制御
    //  ) {
    //  return false;
    //}

    // バックスペースキー制御
    if(keyCode == 8){
      // テキストボックス／テキストエリアを制御する
      var tag = typeof obj.tagName === 'undefined' ? '' : obj.tagName.toUpperCase();
      var type = typeof obj.type === 'undefined' ? '' : obj.type.toUpperCase();
      if ((tag == 'INPUT' && type == 'TEXT')
      	  || tag == 'TEXTAREA'
      	  || (tag == 'INPUT' && type == 'PASSWORD')
        ) {
        //入力可能
        if(!obj.readOnly && !obj.disabled){
          return true;
        }
      }
      return false;
    }

    // Alt + ←制御
    if(altClick && (keyCode == 37 || keyCode == 39)){
      return false;
    }

    // Ctrl + N制御
    if(ctrlClick && keyCode == 78){
      return false;
    }
  });
});

/*
 * 右クリック禁止
 */
//$(function () {
//  $(document).on('contextmenu', function (e) {
//    return false;
//  });
//});


// 2017-03-31 sbc-sagara add str
/*
 * 目標/達成水準行数チェック
 */
function itemlineCheck(str, numX, numY) {
    code = "";
    linecnt = 0;
    //改行コード取得
    if (str.indexOf("\r\n") > -1) {
        code = "\r\n";
    } else if (str.indexOf("\n") > -1) {
        code = "\n";
    } else {
        code = "\r";
    }
    //改行コードで分割
    strline = str.split(code);
    for (j = 0; j < strline.length; j++) {
        if (strline[j].length > numX) {
            linecnt += Math.ceil(strline[j].length / numX) - 1;
        }
    }
    //行数
    linecnt += strline.length;
    if (linecnt > numY) {
        return false
    }
}

/*
 * 文字数・改行エラー 警告のみ、OK時保存処理実行
 */
function showSaveMessageAndAlert(message, msg, val) {
    $('#show_dialog').html("以下の項目の文字数、または改行が多過ぎます。<br />出力時に入力内容が表示しきれない場合があります。<br />" + message);

    // ダイアログを作成
    $('#show_dialog').dialog({
        modal: true,
        title: '確認',
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                //ボタンクリック
                showMessage('確認', msg, val, true);
                return;
            },
            'キャンセル': function () {
                $(this).dialog('close');
                return;
            }
        }
    });
}

/*
 * 文字数・改行エラー 警告のみ、OK時一時保存処理実行
 */
function showSaveMessageAndAlertEx(message) {
    $('#show_dialog').html("以下の項目の文字数、または改行が多過ぎます。<br />出力時に入力内容が表示しきれない場合があります。<br />" + message);

    // ダイアログを作成
    $('#show_dialog').dialog({
        modal: true,
        title: '確認',
        buttons: {
            'OK': function () {
                $(this).dialog('close');
                showMessageEx('途中保存確認', '保存しますか？', 'savebutton', true);
                return;
            },
            'キャンセル': function () {
                $(this).dialog('close');
                return;
            }
        }
    });
}

function isExistStrDate(strDate) {
    if (strDate === '' ) {
        return 0;
    }
    var str = strDate;
    var data = str.split("/");
    return isExistDate(data[0],data[1],data[2])
}
