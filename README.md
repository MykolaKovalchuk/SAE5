Stitch Art Easy! 5
==================

# Cross-stitch pattern scheme generator and editor

https://stitcharteasy.com

If you or someone close to you like cross-stitches embroidery, then you will like Stitch Art Easy!\
It is an editor which will help you to create embroidery pattern schemes from any photos and pictures in a few easy and quick steps.
You just need to select source picture, select desired embroidery size, which threads and how many colors should be used.
And you will receive prepared pattern scheme, which can be used for cross-stitch embroidery reference.
You can print the scheme, save it for later use, or import it into image or PDF file.

![IMAGE](https://raw.githubusercontent.com/MykolaKovalchuk/SAE5/refs/heads/master/Resources/Images/Scrennshots/wizard1.png)\
![IMAGE](https://raw.githubusercontent.com/MykolaKovalchuk/SAE5/refs/heads/master/Resources/Images/Scrennshots/scheme4.png)

Main features:
  - Very simple and intuitive interface;
  - High quality of produced result, including the most precise colors matching;
  - Quick responsive interaction, no need to wait for long time till scheme is regenerated after each change of any parameter;
  - Multiple modes of pattern scheme display, it can also print in all these modes;
  - Favourite threads suites;
  - Export to PDF, Excel, graphical formats.

# Technical details

Written in C# .NET with WinForms.

Principal UI elements (picture boxes and pattern scheme grid) are split into 2 layers:
  - abstract layer with drawing and mouse logic;
  - WinForms wrappers.

This was done for future porting to other platforms.

## Components

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
