﻿using System.IO;

/// <license>
/// This file is part of Ordisoftware Hebrew Pi.
/// Copyright 2025 Olivier Rogier.
/// See www.ordisoftware.com for more information.
/// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
/// If a copy of the MPL was not distributed with this file, You can obtain one at
/// https://mozilla.org/MPL/2.0/.
/// If it is not possible or desirable to put the notice in a particular file,
/// then You may include the notice in a location(such as a LICENSE file in a
/// relevant directory) where a recipient would be likely to look for such a notice.
/// You may add additional accurate notices of copyright ownership.
/// </license>
/// <created> 2025-01 </created>
/// <edited> 2025-01 </edited>
namespace Ordisoftware.Hebrew.Pi;

/// <summary>
/// Provides application's main form.
/// </summary>
/// <seealso cref="T:System.Windows.Forms.Form"/>
partial class MainForm : Form
{

  internal SQLiteNetORM DB { get; private set; }

  private string SQLiteTempDir = @"D:\";

  private PiFirstDecimalsLenght PiFirstDecimalsCount;

  private string DbFilePath = string.Empty;

  #region Singleton

  /// <summary>
  /// Indicates the singleton instance.
  /// </summary>
  static internal MainForm Instance { get; private set; }

  /// <summary>
  /// Static constructor.
  /// </summary>
  static MainForm()
  {
    Instance = new MainForm();
  }

  #endregion

  public MainForm()
  {
    InitializeComponent();
    DoConstructor();
  }

  private void MainForm_Load(object sender, EventArgs e)
  {
    DoFormLoad(sender, e);
    InitializeListBoxCacheSize();
    TimerMemory_Tick(null, null);
  }

  private void InitializeListBoxCacheSize()
  {
    int memTotal = (int)( SystemManager.TotalVisibleMemoryValue / 1024 / 1024 / 1024 );
    int memFree = (int)( SystemManager.PhysicalMemoryFreeValue / 1024 / 1024 / 1024 );
    int indexList = 0;
    for ( int indexStep = 0; indexStep < memTotal * 70 / 100; indexStep += 4 )
    {
      SelectDbCache.Items.Add(indexStep);
      if ( indexStep <= memFree ) indexList++;
    }
    SelectDbCache.SelectedIndex = indexList / 2 - 1;
  }

  private void MainForm_Shown(object sender, EventArgs e)
  {
    DoFormShown(sender, e);
  }

  private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
  {
    DoFormClosing(sender, e);
  }

  private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
  {
    DoFormClosed(sender, e);
  }

  private void ActionExit_Click(object sender, EventArgs e)
  {
    Close();
  }

  private void ActionExit_MouseUp(object sender, MouseEventArgs e)
  {
    if ( e.Button == MouseButtons.Right )
      ActionExit_Click(ActionExit, null);
  }

  internal void EditScreenPosition_Click(object sender, EventArgs e)
  {
    DoScreenPosition(sender, e);
  }

  private void ActionResetWinSettings_Click(object sender, EventArgs e)
  {
    if ( DisplayManager.QueryYesNo(SysTranslations.AskToRestoreWindowPosition.GetLang()) )
      Settings.RestoreMainForm();
  }

  private void ActionShowKeyboardNotice_Click(object sender, EventArgs e)
  {
    Globals.KeyboardShortcutsNotice?.Popup();
  }

  internal void EditDialogBoxesSettings_CheckedChanged(object sender, EventArgs e)
  {
    Settings.SoundsEnabled = EditSoundsEnabled.Checked;
    DisplayManager.AdvancedFormUseSounds = EditSoundsEnabled.Checked;
    DisplayManager.FormStyle = EditUseAdvancedDialogBoxes.Checked
      ? MessageBoxFormStyle.Advanced
      : MessageBoxFormStyle.System;
    DisplayManager.IconStyle = DisplayManager.FormStyle switch
    {
      MessageBoxFormStyle.System => EditSoundsEnabled.Checked
                                    ? MessageBoxIconStyle.ForceInformation
                                    : MessageBoxIconStyle.ForceNone,
      MessageBoxFormStyle.Advanced => MessageBoxIconStyle.ForceInformation,
      _ => throw new AdvNotImplementedException(DisplayManager.FormStyle),
    };
  }

  private void EditShowSuccessDialogs_CheckStateChanged(object sender, EventArgs e)
  {
    Settings.ShowSuccessDialogs = EditShowSuccessDialogs.Checked;
    DisplayManager.ShowSuccessDialogs = EditShowSuccessDialogs.Checked;
  }

  private void ActionPreferences_Click(object sender, EventArgs e)
  {
    bool temp = Globals.IsReadOnly;
    try
    {
      Globals.IsReadOnly = true;
      //PreferencesForm.Run();
      InitializeSpecialMenus();
      InitializeDialogsDirectory();
    }
    catch ( Exception ex )
    {
      ex.Manage();
    }
    finally
    {
      Globals.IsReadOnly = temp;
    }
  }

  private void ActionViewDecode_Click(object sender, EventArgs e)
  {
    SetView(ViewMode.Decode);
  }

  private void ActionViewGrid_Click(object sender, EventArgs e)
  {
    SetView(ViewMode.Grid);
  }

  private void ActionViewPopulate_Click(object sender, EventArgs e)
  {
    SetView(ViewMode.Populate);
  }

  private void ActionViewNormalize_Click(object sender, EventArgs e)
  {
    SetView(ViewMode.Normalize);
  }

  private void ActionViewStatistics_Click(object sender, EventArgs e)
  {
    SetView(ViewMode.Statistics);
  }

  private void ActionDatabaseSetCacheSize_Click(object sender, EventArgs e)
  {
    // TODO
  }

  private void TimerMemory_Tick(object sender, EventArgs e)
  {
    LabelStatusFreeMem.Text = "Free memory: " + SystemManager.PhysicalMemoryFreeValue.FormatBytesSize();
    LabelTitleRight.Text = DB is null
      ? "CLOSED"
      : $"OPENED ({SystemManager.GetFileSize(DbFilePath).FormatBytesSize()})";
  }

  private void TimerBatch_Tick(object sender, EventArgs e)
  {
    LabelStatusTime.Text = Globals.ChronoBatch.Elapsed.AsReadable();
  }

  private void ActionStop_Click(object sender, EventArgs e)
  {
    Globals.CancelRequired = true;
  }

  private void ActionPauseContinue_Click(object sender, EventArgs e)
  {
    Globals.PauseRequired = !Globals.PauseRequired;
    UpdateButtons();
    if ( Globals.PauseRequired )
      Globals.ChronoBatch.Stop();
    else
      Globals.ChronoBatch.Start();
  }

  private void ActionRun_Click(object sender, EventArgs e)
  {
    switch ( Settings.CurrentView )
    {
      case ViewMode.Populate:
        //if ( !DisplayManager.QueryYesNo("Empty and create data?") ) return;
        string fileName = Path.Combine(Globals.DocumentsFolderPath, PiFirstDecimalsCount.ToString()) + ".txt";
        DoBatch(() => DoActionPopulate(fileName));
        break;
      case ViewMode.Normalize:
        //if ( !DisplayManager.QueryYesNo("Start reducing repeating motifs?") ) return;
        DoBatch(() => DoActionNormalize(0));
        break;
      default:
        throw new AdvNotImplementedException(Settings.CurrentView);
    }
  }

  private async Task DoBatch(Action action)
  {
    try
    {
      ClearStatusBar();
      SetBatchState(true);
      UpdateButtons();
      Globals.ChronoBatch.Restart();
      TimerBatch_Tick(null, null);
      TimerBatch.Enabled = true;
      await Task.Run(async () => action());
    }
    finally
    {
      Globals.ChronoBatch.Stop();
      TimerBatch.Enabled = false;
      SetBatchState(false);
      UpdateButtons();
    }
  }

  private void SetBatchState(bool active)
  {
    Globals.IsInBatch = active;
    Globals.PauseRequired = false;
    Globals.CancelRequired = false;
  }

  private async Task<bool> CheckIfBatchCanContinue()
  {
    if ( Globals.CancelRequired ) return false;
    while ( Globals.PauseRequired && !Globals.CancelRequired )
      await Task.Delay(500);
    return true;
  }

  //private void Grid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
  //{
  //  using var brush = new SolidBrush(Grid.RowHeadersDefaultCellStyle.ForeColor);
  //  e.Graphics.DrawString(( e.RowIndex + 1 ).ToString(),
  //                        e.InheritedRowStyle.Font,
  //                        brush,
  //                        e.RowBounds.Location.X + 10,
  //                        e.RowBounds.Location.Y + 4);
  //}

  //private void Grid_KeyDown(object sender, KeyEventArgs e)
  //{
  //  if ( e.Control && e.KeyCode == Keys.C && Grid.SelectedCells.Count > 0 )
  //  {
  //    var builder = new StringBuilder();
  //    foreach ( DataGridViewCell cell in Grid.SelectedCells )
  //    {
  //      builder.Append(cell.Value.ToString());
  //      if ( cell.ColumnIndex == Grid.Columns.Count - 1 )
  //        builder.AppendLine();
  //      else
  //        builder.Append("\t");
  //    }
  //    Clipboard.SetText(builder.ToString());
  //  }
  //}

  private void SelectFileName_SelectedIndexChanged(object sender, EventArgs e)
  {
    PiFirstDecimalsCount = (PiFirstDecimalsLenght)SelectFileName.SelectedItem;
    UpdateButtons();
  }

  private void SelectDbCache_SelectedIndexChanged(object sender, EventArgs e)
  {
    SetDbCache();
  }

  private void SetDbCache()
  {
    if ( DB is not null ) DB.SetCacheSize((int)SelectDbCache.SelectedItem * 1024 * 1024);
  }

  private void ActionDbOpen_Click(object sender, EventArgs e)
  {
    DbFilePath = Path.Combine(Globals.DatabaseFolderPath, PiFirstDecimalsCount.ToString()) + Globals.DatabaseFileExtension;
    LabelTitleCenter.Text = Path.GetFileName(DbFilePath);
    DB = new SQLiteNetORM(DbFilePath);
    if ( SQLiteTempDir.Length > 0 )
      DB.SetTempDir(SQLiteTempDir);
    DB.CreateTable<DecupletRow>();
    DB.CreateTable<IterationRow>();
    SetDbCache();
    UpdateButtons();
    TimerMemory_Tick(null, null);
  }

  private void ActionDbClose_Click(object sender, EventArgs e)
  {
    if ( DB is null ) return;
    DB.Close();
    DB.Dispose();
    DB = null;
    UpdateButtons();
    TimerMemory_Tick(null, null);
  }

}
