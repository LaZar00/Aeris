REM  SAMPLE: aeris -d D:\Coding\_Aeris\_UNCFIELDS\blin2_i D:\Coding\_Aeris\_ALLBASEUNSWIZZLED -v

@echo off

set "fieldname=datiao_8"
set "fieldfolder=D:\Coding\_Aeris\_UNCFIELDS"
set "inputfolder=D:\Coding\_Aeris\_ALLBASEUNSWIZZLED"
set "outputfolder=D:\Coding\_Aeris\_ALLBASESWIZZLED"

echo Input Folder:  %inputfolder%
echo Output Folder: %outputfolder%


if NOT EXIST %fieldfolder%\%fieldname% (
	echo(
	echo Field: %fieldfolder%\%fieldname% does not exists.
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


echo Checking Field: %fieldname%
	
if NOT EXIST %outputfolder%\%fieldname% (
	echo Creating new Folder Field: %fieldname%
	mkdir %outputfolder%\%fieldname%
) else (
	echo Deleting files of Folder Field: %fieldname%
	erase %outputfolder%\%fieldname% /Q
)
	
echo Processing Field: %fieldname%
aeris -b %fieldfolder%\%fieldname% %inputfolder%\%fieldname% %outputfolder%\%fieldname% -v


:exit
@echo on