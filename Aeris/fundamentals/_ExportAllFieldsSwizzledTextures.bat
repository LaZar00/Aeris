REM  SAMPLE: aeris -d D:\Coding\_Aeris\_UNCFIELDS\blin2_i D:\Coding\_Aeris\_ALLBASEUNSWIZZLED -v

@echo off

set "inputfolder=D:\Coding\_Aeris\_UNCFIELDS"
set "outputfolder=D:\Coding\_Aeris\_ALLBASETEXTURES"

echo Input Folder:  %inputfolder%
echo Output Folder: %outputfolder%

if NOT EXIST %inputfolder% (
	echo( 
	echo Folder: %inputfolder% does not exists.
	goto exit
)

if NOT EXIST %outputfolder% (
	echo(
	echo Folder: %outputfolder% does not exists.
	goto exit
)

for %%f in (%inputfolder%\*) do (

	echo.
	echo =================== INIT =========================
	echo Checking Field: %%~nf
	
	if NOT EXIST %outputfolder%\%%~nf (
		echo Creating new Folder Field: %%~nf
		mkdir %outputfolder%\%%~nf
	) else (
		echo Deleting files of Folder Field: %%~nf
		erase %outputfolder%\%%~nf\* /Q
	)
	
	echo Processing Field: %%~nf
	aeris -x %inputfolder%\%%~nf %outputfolder%\%%~nf -v
	echo =================== END =========================
)

:exit
@echo on