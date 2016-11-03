dir_project=$1
echo $dir_project

cd $dir_project

unity_target="Unity-iPhone"
echo $unity_target

unity_project=$unity_target.xcodeproj
echo $unity_project

dir_build=$dir_project/build
echo $dir_build

if [ -d $dir_build ]; then
    rm -rf $dir_build
fi

dir_product_release=$dir_build/Release-iphoneos
echo $dir_product_release

dir_product_debug=$dir_build/Debug-iphoneos
echo $dir_product_debug

if [ -!f $unity_project ]; then
    exit 1
fi

xcodebuild -project $unity_project -target $unity_target OBJROOT=$dir_build SYMROOT=$dir_build

cd $dir_build

if [ -d $dir_product_release ]; then
    cd $dir_product_release
elif [ -d $dir_product_debug ]; then
   	cd $dir_product_debug
fi

temp_folder="Payload"
echo $temp_folder

if [ -d $temp_folder ]; then
    rm -rf $temp_folder
fi

mkdir $temp_folder

product_name=$2
echo $product_name

app=$product_name.app
echo $app

if [ -!f $app ]; then
    exit 1
fi

cp -rf $app $temp_folder/$app

ipa=$product_name.ipa
echo $ipa

if [ -f $ipa ]; then
    rm -rf $ipa
fi

zip -r $ipa $temp_folder

open .
