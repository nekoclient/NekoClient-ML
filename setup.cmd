@echo off

if [%1] == [] goto usage

for %%I in (%1) do set TargetDirName=%%~nxI

if [%TargetDirName%]==[MelonLoader] (
	@echo Got ML folder!
	if exist %1\\Managed\\*.dll (
		call mklink /d %~dp0\\_deps\\Managed %1\\Managed
		if %ERRORLEVEL% == 0 goto :succeeded
		@echo Please turn on Developer mode or run this in an admin prompt, symbolic link creation failed.
		goto :end

		:succeeded
		call mklink %~dp0\\_deps\\MelonLoader.dll %1\\MelonLoader.dll
		call mklink %~dp0\\_deps\\MelonLoader.ModHandler.dll %1\\MelonLoader.ModHandler.dll
		@echo Symbolic links created! References in the csproj will work fine now!
		goto :end
	) else (
		@echo Managed directory is empty. Run the game at least once before executing this script.
	)
) else (
	@echo Folder wasn't MelonLoader!
)

goto :end

:usage
@echo Drag and drop the MelonLoader folder onto me.
goto :end

:end
pause