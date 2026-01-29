using System.Configuration;

namespace ISCAttendanceMgmtSystem.COM.Util.Config {
    /// <summary>
    /// ============================================================================== 
    /// Iwai Project                                  CopyRight(C)　SBC 
    /// ------------------------------------------------------------------------------ 
    /// 機能名　　:　Web.Config操作クラス
    /// ファイル名:　WebConfig.cs
    /// 処理区分　:　共通ユーティリティ
    /// 作成日　　:　2015/02/21 
    /// 作成者　　:　SBC y.katoh
    /// バージョン:　1.0 
    /// 機能概要　:  Web.Configからシステムコンフィグデータを取得する
    ///  
    /// ============================================================================== 
    /// </summary>
    public static class WebConfig {
        /// <summary>
        ///  コンフィグファイル読み込み
        /// </summary>
        /// <returns>コンフィグ</returns>
        public static Configuration GetConfigFile() {
            string cofigpath = System.AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            cofigpath = cofigpath == "" ?
                    System.IO.Directory.GetParent(System.IO.Path.GetDirectoryName(System.Reflection.Assembly
                        .GetExecutingAssembly().GetName().CodeBase.Substring(8))).FullName + "\\Config\\Web.config"
                    : cofigpath;

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = cofigpath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            return config;
        }
    }
}
