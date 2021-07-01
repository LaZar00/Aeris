REM  SAMPLE: aeris -u D:\Coding\_Aeris\_UNCFIELDS\blin2_i D:\Coding\_Aeris\_ANIMS D:\Coding\_Aeris\_ANIMSUNSWIZZLED -v

@echo off

set "fieldsfolder=D:\Coding\_Aeris\_UNCFIELDS"
set "inputfolder=D:\Coding\_Aeris\_ANIMSUNSWIZZLED"
set "outputfolder=D:\Coding\_Aeris\_ANIMSSWIZZLED"

echo Fields Folder: %fieldsfolder%
echo Input Folder:  %inputfolder%
echo Output Folder: %outputfolder%

if NOT EXIST %fieldsfolder% (
	echo( 
	echo Fields Folder: %fieldsfolder% does not exists.
	goto exit
)

if NOT EXIST %inputfolder% (
	echo( 
	echo Unswizzled Hashed Anims Folder: %inputfolder% does not exists.
	goto exit
)

if NOT EXIST %outputfolder% (
	echo(
	echo Swizzled Hashed Anims Folder: %outputfolder% does not exists.
	goto exit
)

for /D %%d in (%inputfolder%\*) do (

	echo.
	echo =================== INIT =========================
	echo Checking Field: %%~nd
	
	if NOT EXIST %outputfolder%\%%~nd (
		echo Creating new Folder Field: %%~nd
		mkdir %outputfolder%\%%~nd
	) else (
		echo Deleting files of Output Folder Field: %%~nd
		erase %outputfolder%\%%~nd\* /Q
	)
	
	echo Processing Field: %%~nd
	aeris -s %fieldsfolder%\%%~nd %inputfolder%\%%~nd %outputfolder%\%%~nd -v
	echo =================== END =========================
)

:exit
@echo on