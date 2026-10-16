@echo on
if "%MSFS_SDK%"=="" (
    echo MSFS_SDK environment variable is not set. Install the MSFS SDK and set MSFS_SDK to its root folder.
    exit /b 1
)

"%MSFS_SDK%\Tools\bin\fspackagetool.exe" "Build\stol-training-panel.xml" -nopause
if errorlevel 1 (
    echo fspackagetool failed.
    exit /b 1
)

rem fspackagetool finishes its actual build asynchronously (it launches the sim to
rem validate/compile), so the .spb may not exist the instant the tool returns.
rem Poll for it for up to ~60 seconds before giving up.
set SPB_SRC=Build\Packages\stol-training-panel\Build\stol-training-panel.spb
set WAIT_COUNT=0
:waitspb
if exist "%SPB_SRC%" goto gotspb
set /a WAIT_COUNT+=1
if %WAIT_COUNT% GTR 30 (
    echo Timed out waiting for %SPB_SRC% to appear.
    echo fspackagetool may still be building in the background - check for a FlightSimulator window,
    echo then re-run build.bat once it closes, or copy the .spb manually when it appears.
    exit /b 1
)
timeout /t 2 /nobreak >nul
goto waitspb

:gotspb
copy /Y "%SPB_SRC%" "InGamePanels\stol-training-panel.spb"
echo Done. Copy this whole STOL_Training_Panel folder (renamed if you like) into your Community folder,
echo but do NOT include the Build\ subfolder.
