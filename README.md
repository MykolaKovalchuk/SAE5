Stitch Art Easy! 5
==================

Cross-stitch pattern scheme generator and editor.
https://stitcharteasy.com

Written in C# .NET with WinForms.

Principal UI elements (picture boxes and pattern scheme grid) are split into 2 layers:
abstract layer with drawing and mouse logic, and WinForms wrappers.
This was done for future porting to other platforms.

## Assemblies

**Ravlyk.Common** - base common classes (disposables and undo-redo manager).

**Ravlyk.Drawing** - IndexedImage, Color, Pallette classes. Custom imaging library with direct access to pixels array for fast operations.

**Ravlyk.Drawing.ImageProcessing** - image transformations, most with multi-threading.

**Ravlyk.Adopted** - cloned sources of external libraries with some changes.

**Ravlyk.SAE.Resources** - shared resources.

**Ravlyk.SAE.Drawing**
 - extension of IndexedImage for cross-stitch related functionality;
 - scheme serialization;
 - painters and exporters;
 - abstart level of UI controls with all logic (drawing, mouse, undo-redo);
 - new scheme wizard.

**Ravlyk.UI** - concrete UI controls (only for WinForms).

**Apps\SAE5.Win** - WinForm GUI application.
