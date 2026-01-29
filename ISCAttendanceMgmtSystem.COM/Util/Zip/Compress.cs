using Ionic.Zip;
using System.Collections;

namespace ISCAttendanceMgmtSystem.COM.Util.Zip {
    /// <summary>
    /// ============================================================================== 
    /// IWAI Project                                  CopyRight(C)　SBC. 
    /// ------------------------------------------------------------------------------ 
    /// サイト　　:　職能判定システム 
    /// 機能名　　:　共通圧縮ロジック
    /// ファイル名:　Compress.cs
    /// 処理区分　:　共通ロジック
    /// 作成日　　:　2015/03/12 
    /// 作成者　　:　SBC Y.Katoh 
    /// バージョン:　1.0 
    /// 機能概要　:  各作成帳票、インターフェースファイルを圧縮する
    ///  
    /// ============================================================================== 
    /// </summary>
    public class Compress {

        /// <summary>
        /// zipファイル生成
        /// </summary>
        /// <param name="zipFname">作成ファイル名</param>
        /// <param name="zipDirPath">作成先パス</param>
        /// <param name="filePathList">圧縮対象ファイルパスリスト</param>
        /// <returns>zip圧縮先フルパス</returns>
        public string CreateZipFile(string zipFname, string zipDirPath, IList filePathList) {
            //zipフルパス設定取得
            string zipFullPath = zipDirPath + zipFname;

            //出力されるリストフォルダからファイル一覧を取得
            string[] files = System.IO.Directory.GetFiles(zipDirPath, "*", System.IO.SearchOption.AllDirectories);

            //圧縮したファイルを格納する書庫内のディレクトリのパス
            string directoryPathInArchive = "";
            //サブフォルダも対象とするか
            bool recurseDirectories = false;

            //ZipFileを作成する
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile()) {
                //4GB以上を作成する為。
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

                //作ろうとしているzipファイルが存在したら削除
                if (System.IO.File.Exists(zipFullPath)) {
                    System.IO.File.Delete(zipFullPath);
                }

                //shift_jisしたら文字化けした。
                //zip.ProvisionalAlternateEncoding = System.Text.Encoding.GetEncoding("shift_jis");   //文字コードの設定
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                //zip.AlternateEncoding = System.Text.Encoding.GetEncoding("utf-8");
                zip.AlternateEncoding = System.Text.Encoding.GetEncoding("shift_jis");

                //指定されたファイルを追加する
                //zip.AddSelectedFiles(selectionCriteria, zipDirPath, directoryPathInArchive, recurseDirectories);
                foreach (string file in filePathList) {
                    zip.AddSelectedFiles(file, zipDirPath, directoryPathInArchive, recurseDirectories);
                }

                //ZIP書庫を作成する
                zip.Save(zipFullPath);
            }

            //ファイルのフルパスをリターン
            return zipFullPath;
        }

        /// <summary>
        /// zipファイル生成
        /// </summary>
        /// <param name="zipFname">作成ファイル名</param>
        /// <param name="zipDirPath">作成先パス</param>
        /// <param name="dirPath">圧縮対象ディレクトリパス</param>
        /// <returns>zip圧縮先フルパス</returns>
        public string CreateZipFile(string zipFname, string zipDirPath, string dirPath) {
            //zipフルパス設定取得
            string zipFullPath = zipDirPath + zipFname;

            //ZipFileを作成する
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile()) {
                //4GB以上を作成する為。
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

                //作ろうとしているzipファイルが存在したら削除
                if (System.IO.File.Exists(zipFullPath)) {
                    System.IO.File.Delete(zipFullPath);
                }

                //shift_jisしたら文字化けした。
                //zip.ProvisionalAlternateEncoding = System.Text.Encoding.GetEncoding("shift_jis");   //文字コードの設定
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                //zip.AlternateEncoding = System.Text.Encoding.GetEncoding("utf-8");
                zip.AlternateEncoding = System.Text.Encoding.GetEncoding("shift_jis");

                //指定されたファイルを追加する
                zip.AddDirectory(dirPath);

                //ZIP書庫を作成する
                zip.Save(zipFullPath);
            }

            //ファイルのフルパスをリターン
            return zipFullPath;
        }

    }
}
