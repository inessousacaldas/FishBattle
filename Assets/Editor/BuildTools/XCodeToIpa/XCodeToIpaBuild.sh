#!/bin/bash
# @Author: Fly
# @Date:   2015-12-26 15:47:09
# @Last Modified by:   cilu2
# @Last Modified time: 2015-12-29 09:23:10

#第一个是log路径
echo $1
echo $2
echo $3
echo $4
echo $5

#工程路径
project_path=$2

#IPA名称
ipa_name=$3

#签证
prov_uuid=$4

#配置
plist_path=$5

#build文件夹路径
build_path=${project_path}/build

archive_path=${build_path}/${ipa_name}.xcarchive
app_path=${build_path}/Release-iphoneos/${ipa_name}.app
ipa_path=${build_path}/${ipa_name}.ipa

cd $project_path

#清理#
# xcodebuild  clean

#编译工程
xcodebuild -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -destination generic/platform=ios archive -archivePath "$archive_path" DEVELOPMENT_TEAM="P64T2GRG6H"

#导出
# xcodebuild -exportArchive -archivePath "$archive_path" -exportPath "$ipa_path" -exportFormat ipa -exportProvisioningProfile "$prov_uuid"
xcodebuild -exportArchive -archivePath "$archive_path" -exportPath "$build_path" -exportOptionsPlist "$plist_path"
mv "${build_path}/Unity-iPhone.ipa" "$ipa_path"