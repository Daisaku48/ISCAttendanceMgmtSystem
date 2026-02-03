# ISC Attendance Management System - Solution Template

本リポジトリは、今後の Web アプリ開発に再利用できるように、既存のソリューション構成をテンプレート化するための指針をまとめたものです。
現状のプロジェクト構成を維持しつつ、再利用の際に必要となる整理・置換ポイントを明確にします。

## 目的
- 既存の 3 層構成（COM/BL/Web）をベースにしたテンプレート化
- 追加開発時の初期コスト削減
- 新規 Web アプリの起動手順・命名規約の統一

## ディレクトリ構成（テンプレート）
```
ISCAttendanceMgmtSystem.sln
ISCAttendanceMgmtSystem.COM   # 共通・モデル層
ISCAttendanceMgmtSystem.BL    # ビジネスロジック層
ISCAttendanceMgmtSystem.Web   # Web UI 層
```

## 新規アプリ作成時の基本手順
1. **ソリューション複製**
   - リポジトリごとコピーし、フォルダ名を新プロジェクト名に変更します。
2. **ソリューション/プロジェクト名の置換**
   - `ISCAttendanceMgmtSystem` の名称を新プロジェクト名に置換します。
3. **名前空間の整理**
   - COM/BL/Web の名前空間を新しい名称に合わせて整理します。
4. **設定ファイルの確認**
   - `Web.config` の接続文字列・アプリ設定を更新します。
5. **ビルド・起動確認**
   - Visual Studio で全体をビルドし、Web プロジェクトの起動確認を行います。

## 詳細ガイド
- 具体的なテンプレート化手順・チェックリストは `docs/solution-template.md` を参照してください。
