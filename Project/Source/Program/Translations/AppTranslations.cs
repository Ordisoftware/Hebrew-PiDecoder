﻿/// <license>
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
/// <created> 2025-01-12 </created>
/// <edited> 2025-01-14 </edited>
namespace Ordisoftware.Hebrew.Pi;

static partial class AppTranslations
{

  static public readonly TranslationsDictionary ApplicationDescription = new()
  {
    [Language.EN] = "Pi decimal decoder with Hebrew Gematria",
    [Language.FR] = "Décodeur des décimales de Pi avec la Gematria hébraïque"
  };

  static public readonly TranslationsDictionary AskToCreateNewDatabase = new()
  {
    [Language.EN] = "Do you want to create a new database which will replace the actual?",
    [Language.FR] = "Voulez-vous créer une nouvelle base de données qui remplacera l'actuelle ?"
  };

  static public readonly TranslationsDictionary AskToReplaceDatabase = new()
  {
    [Language.EN] = "Do you want to replace the actual database with the selected backup?" + Globals.NL2 + "{0}",
    [Language.FR] = "Voulez-vous remplacer la base de données actuelle avec l'archive sélectionnée ?" + Globals.NL2 + "{0}"
  };

  static public readonly TranslationsDictionary AskToBackupDatabaseBeforeReplace = new()
  {
    [Language.EN] = "Do you want to backup database before replace it?",
    [Language.FR] = "Voulez-vous archiver la base de données avant de la remplacer ?"
  };

  static public readonly TranslationsDictionary NoSearchResultFound = new()
  {
    [Language.EN] = "No result found",
    [Language.FR] = "Aucun résultat trouvé"
  };

  static public readonly NullSafeDictionary<ViewMode, TranslationsDictionary> ViewPanelTitle = new()
  {
  };

  // Text file
  //static public string File_NoRepeatedText = "Aucun répété.";
  //static public string File_PrefixText = "Sur l'échantillon donné, la théorie des répétés ajoutés aux positions";
  //static public string File_OkText = $"{File_PrefixText} fonctionne.";
  //static public string File_NotOkText = $"{File_PrefixText} ne fonctionne pas.";
  //static public string File_SavedFixedText = "Les groupes dupliquée ont été corrigés et le fichier reconstruit.";

  // Database
  static public string CreateDataProgress = "{0} inserted";
  static public string RepeatedAtIteration = $"repeating motifs at iteration #{{0}}{Globals.NL2}";
  static public string PreviousAndCurrentCount = $"Previous: {{1}}{Globals.NL}Current: {{2}}{Globals.NL2}";
  static public string LessAtIteration = $"There are less {RepeatedAtIteration}{Globals.NL2}";
  static public string MoreAtIteration = $"There are more {RepeatedAtIteration}{Globals.NL2}";
  static public string StartNextIteration = "Start the next iteration?";
  static public string AskStartNextIfLess = $"{LessAtIteration}{PreviousAndCurrentCount}{StartNextIteration}";
  static public string AskStartNextIfMore = $"{MoreAtIteration}{PreviousAndCurrentCount}{StartNextIteration}";

  static public string NoRepeatedText = "There is no repeating motif at iteration {0}";
  static public string IterationText = "Iteration {0} : {1} repeating";
  static public string CountingText = "Counting...";
  static public string CountedText = "Counted";
  static public string UpdatingText = "Adding position...";
  static public string CommittingText = "Committing...";
  static public string IndexingText = "Indexing...";
  static public string FinishedText = "Finished";
  static public string CanceledText = "Canceled";

  static public string EmptyingTablesText = "Emptying tables...";
  static public string PopulatingText = "Populating...";
  static public string PopulatingAndRemainingText = "Populating... ({0} remaining before indexing)";

}
