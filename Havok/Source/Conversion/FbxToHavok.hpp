#pragma once

#include "../Havok/HavokHeaders.hpp"
#include "../FBX/FbxUtils.hpp"
#include <fbxsdk.h>
#include <string>
#include <vector>

class FbxToHavok
{
public:
	static int ConvertSkeleton(FbxScene* sourceScene, hkaSkeleton* targetSkeleton, const std::vector<std::string>& boneOrder);

	static int ConvertAnimation(FbxScene* sourceScene, FbxAnimStack* sourceAnim, hkaSkeleton* sourceSkeleton, hkaInterleavedUncompressedAnimation* targetAnimation, hkaAnimationBinding* targetBinding, const std::vector<std::string>& excludeBones);
};

