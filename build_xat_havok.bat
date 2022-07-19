CALL "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\Tools\VsDevCmd.bat"

msbuild Havok\XATHavokInterop.sln /t:build /p:Configuration=Release

rmdir /S /Q "XAT\Lib\XATHavokInterop"

xcopy "Havok\bin\Release\" "XAT\Lib\XATHavokInterop\" /i /c /k /e /r /y