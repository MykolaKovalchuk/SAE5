# SAE5
Stitch Art Easy! 5

Cross-stitch scheme pattern generator and editor.

Written in C# .NET.
UI uses WinForms.

Principal UI elements (picture boxes and pattern scheme grid) are split into 2 layers:
abstract layer with drawing and mouse logic, and WinForms wrappers.
This was done for future porting to other platforms.

Ravlyk.Common - base common classes (disposables and undo-redo manager).
Ravlyk.Drawing - IndexedImage class.
Ravlyk.Drawing.ImageProcessing - image transformations.

Ravlyk.Adopted - cloned sources of external libraries with some changes.

Ravlyk.SAE.Resources - SAE5 resources.
Ravlyk.SAE.Drawing - abstart level of UI controls with all lagic (drawing, mouse, undo-redo).

Ravlyk.UI - concrete UI (for now only WinForms).

Apps\SAE5.Win - WinForm GUI application.
