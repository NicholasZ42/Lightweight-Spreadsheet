// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1130:Use lambda syntax", Justification = "Delegate is what was taught in class.", Scope = "member", Target = "~E:SpreadsheetEngine.Cell.PropertyChanged")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Class assignment.", Scope = "member", Target = "~F:SpreadsheetEngine.Cell.text")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Class assignment.", Scope = "member", Target = "~F:SpreadsheetEngine.Cell.value")]
[assembly: SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1130:Use lambda syntax", Justification = "Delegate is what was taught in class.", Scope = "member", Target = "~E:SpreadsheetEngine.Spreadsheet.CellPropertyChanged")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Needed for inheriting classes.", Scope = "member", Target = "~F:SpreadsheetEngine.ExpressionTreeNode.right")]
[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Needed for inheriting classes.", Scope = "member", Target = "~F:SpreadsheetEngine.ExpressionTreeNode.left")]
[assembly: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:File should have header", Justification = "Doesn't need one.")]
[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "Name in assignment.", Scope = "member", Target = "~F:SpreadsheetEngine.Cell.BGColor")]
[assembly: SuppressMessage("Naming", "VSSpell001:Spell Check", Justification = "Spelling correct", Scope = "member", Target = "~M:SpreadsheetEngine.CellTextChangeCommand.Unexecute")]
[assembly: SuppressMessage("Naming", "VSSpell001:Spell Check", Justification = "Spelling correct", Scope = "member", Target = "~M:SpreadsheetEngine.BackgroundChangeCommand.Unexecute")]
