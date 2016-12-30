MDTOOL_PATH="/usr/local/lib/monodevelop/bin/mdtool.exe"

rm -rf build-mpacks
rm -rf build

xbuild /p:OutputPath=build /p:Configuration=Release /t:Rebuild

mkdir build-mpacks

cd build-mpacks

mono $MDTOOL_PATH setup pack ../build/MonoDevelop.Debugger.Soft.Unity.dll ../UnityUtilities/build/UnityUtilities.dll

cd ..
