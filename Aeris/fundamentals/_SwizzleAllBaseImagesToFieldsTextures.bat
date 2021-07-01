REM  SAMPLE: aeris -d D:\Coding\_Aeris\_UNCFIELDS\blin2_i D:\Coding\_Aeris\_ALLBASEUNSWIZZLED -v

@echo off

set "fieldfolder=D:\Coding\_Aeris\_UNCFIELDS"
set "inputfolder=D:\Coding\_Aeris\_ALLBASEUNSWIZZLED"
set "outputfolder=D:\Coding\_Aeris\_ALLBASESWIZZLED"

echo Field Folder:  %fieldfolder%
echo Input Folder:  %inputfolder%
echo Output Folder: %outputfolder%


if NOT EXIST %fieldfolder% (
	echo(
	echo Field: %fieldfolder% does not exists.
	goto exit
)

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

for %%f in (%fieldfolder%\*) do (

	echo.
	echo =================== INIT =========================
	echo Checking Field: %%~nf
	
	if NOT EXIST %outputfolder%\%%~nf (
		echo Creating new Folder Field: %%~nf
		mkdir %outputfolder%\%%~nf
	) else (
		echo Deleting files of Folder Field: %%~nf
		erase %outputfolder%\%%~nf /Q
	)
	
	echo Processing Field: %%~nf
	aeris -b %fieldfolder%\%%~nf %inputfolder%\%%~nf %outputfolder%\%%~nf -v
	echo =================== END =========================
)

:exit
@echo on